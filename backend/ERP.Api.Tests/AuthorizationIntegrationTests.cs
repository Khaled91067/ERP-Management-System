using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ERP.Api;
using ERP.Domain.Identity.Permissions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace ERP.Api.Tests
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.Request.Headers.ContainsKey("TestRole"))
                return Task.FromResult(AuthenticateResult.Fail("No role"));

            var role = Context.Request.Headers["TestRole"].ToString();
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "Test User"),
                new(ClaimTypes.Role, role)
            };

            var permissions = Permissions.GetDefaultPermissions(role);
            foreach (var perm in permissions)
            {
                claims.Add(new Claim("permission", perm));
            }

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class AuthorizationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AuthorizationIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:RedisConnection"] = ""
                    });
                });

                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                });
            });
        }

        [Fact]
        public async Task AnonymousUser_CannotAccessProtectedApi()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/admin/users");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("Sales", "/api/admin/users", HttpStatusCode.Forbidden)]
        [InlineData("HR", "/api/admin/roles", HttpStatusCode.Forbidden)]
        [InlineData("Sales", "/api/departments", HttpStatusCode.Forbidden)]
        [InlineData("Warehouse", "/api/employees", HttpStatusCode.Forbidden)]
        [InlineData("Finance", "/api/admin/roles", HttpStatusCode.Forbidden)]
        public async Task WrongRole_ReturnsForbidden(string role, string url, HttpStatusCode expectedStatus)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            client.DefaultRequestHeaders.Add("TestRole", role);

            var response = await client.GetAsync(url);
            Assert.Equal(expectedStatus, response.StatusCode);
        }

        [Theory]
        [InlineData("Administrator", "/api/admin/users")]
        [InlineData("Sales", "/api/orders")]
        [InlineData("Purchasing", "/api/purchase-orders")]
        [InlineData("Warehouse", "/api/products")]
        [InlineData("Finance", "/api/invoices")]
        [InlineData("HR", "/api/employees")]
        public async Task CorrectRole_CanAccessModule(string role, string url)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            client.DefaultRequestHeaders.Add("TestRole", role);

            var response = await client.GetAsync(url);
            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Sales_CanReadProducts_ButCannotAdjustStockOrDelete()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            client.DefaultRequestHeaders.Add("TestRole", "Sales");

            // Sales can read products
            var getResponse = await client.GetAsync("/api/products");
            Assert.NotEqual(HttpStatusCode.Unauthorized, getResponse.StatusCode);
            Assert.NotEqual(HttpStatusCode.Forbidden, getResponse.StatusCode);

            // Sales cannot adjust stock
            var patchResponse = await client.PatchAsJsonAsync("/api/products/1/stock", new { QuantityChange = 5 });
            Assert.Equal(HttpStatusCode.Forbidden, patchResponse.StatusCode);

            // Sales cannot delete product
            var deleteResponse = await client.DeleteAsync("/api/products/1");
            Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task Sales_CanReadInvoices_ButCannotPayOrDelete()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            client.DefaultRequestHeaders.Add("TestRole", "Sales");

            // Sales can read invoices
            var getResponse = await client.GetAsync("/api/invoices");
            Assert.NotEqual(HttpStatusCode.Unauthorized, getResponse.StatusCode);
            Assert.NotEqual(HttpStatusCode.Forbidden, getResponse.StatusCode);

            // Sales cannot pay invoices
            var payResponse = await client.PutAsJsonAsync("/api/invoices/1/pay", new { InvoiceId = 1, Amount = 100m, PaymentDate = System.DateTime.UtcNow });
            Assert.Equal(HttpStatusCode.Forbidden, payResponse.StatusCode);

            // Sales cannot delete invoices
            var deleteResponse = await client.DeleteAsync("/api/invoices/1");
            Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task Finance_CanPayInvoices_ButCannotDeleteCustomers()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            client.DefaultRequestHeaders.Add("TestRole", "Finance");

            // Finance can read customers
            var getCustResponse = await client.GetAsync("/api/customers");
            Assert.NotEqual(HttpStatusCode.Unauthorized, getCustResponse.StatusCode);
            Assert.NotEqual(HttpStatusCode.Forbidden, getCustResponse.StatusCode);

            // Finance cannot delete customer
            var deleteCustResponse = await client.DeleteAsync("/api/customers/1");
            Assert.Equal(HttpStatusCode.Forbidden, deleteCustResponse.StatusCode);
        }

        [Fact]
        public async Task Warehouse_CanAdjustStock_ButCannotAccessOrders()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            client.DefaultRequestHeaders.Add("TestRole", "Warehouse");

            // Warehouse cannot access orders
            var ordersResponse = await client.GetAsync("/api/orders");
            Assert.Equal(HttpStatusCode.Forbidden, ordersResponse.StatusCode);
        }
    }
}
