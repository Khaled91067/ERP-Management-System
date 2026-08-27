using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ERP.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
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
            var claims = new[] { new Claim(ClaimTypes.Name, "Test User"), new Claim(ClaimTypes.Role, role) };
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
        [InlineData("Purchasing", "/api/purchaseorders")]
        [InlineData("Warehouse", "/api/products")]
        [InlineData("Finance", "/api/invoices")]
        [InlineData("HR", "/api/employees")]
        public async Task CorrectRole_CanAccessModule(string role, string url)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            client.DefaultRequestHeaders.Add("TestRole", role);
            
            var response = await client.GetAsync(url);
            // It might return 404 Not Found or 200 OK depending on db, but not 401/403.
            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
