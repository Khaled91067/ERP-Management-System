

namespace ERP.Infrastructure
{
    using System.Text;

    using ERP.Application.Abstractions;
    using ERP.Application.Abstractions.Authentication;
    using ERP.Application.Abstractions.Caching;
    using ERP.Application.Abstractions.Repositories;
    using ERP.Infrastructure.Authentication;
    using ERP.Infrastructure.Caching;
    using ERP.Infrastructure.Persistence;
    using ERP.Infrastructure.Persistence.Repositories;
    using ERP.Infrastructure.Persistence.Seeding;
    using ERP.Infrastructure.Repositories;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    using StackExchange.Redis;

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options
                    .UseSqlServer(configuration
                    .GetConnectionString("DefaultConnection")));

            var redisConnectionString = configuration.GetConnectionString("RedisConnection");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                var multiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
                services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            }
            services.Configure<CacheSettings>(configuration.GetSection("Cache"));
            services.AddSingleton<ICacheService, RedisCacheService>();

            services.AddScoped(typeof(IGenericRepository<> ),typeof(GenericRepository<> ));

            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddHttpContextAccessor();
            services.AddScoped<ERP.Application.Abstractions.Common.ICurrentUserService, ERP.Infrastructure.Services.CurrentUserService>();

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));



            services.AddScoped<ITokenService, TokenService>();


            var jwtSettings = configuration.GetSection("Jwt")
                .Get<JwtSettings>() ?? throw new InvalidOperationException("JWT settings are missing.");

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
            
                            ValidIssuer = jwtSettings.Issuer,
                            ValidAudience = jwtSettings.Audience,
            
                            IssuerSigningKey =
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                        };
                });

            services.AddAuthorization();




            services.AddScoped<DbInitializer>();
            services.AddScoped<ISeeder, RoleSeeder>();
            services.AddScoped<ISeeder, UserSeeder>();
            services.AddScoped<ISeeder, CategorySeeder>();
            services.AddScoped<ISeeder, ProductSeeder>();
            services.AddScoped<ISeeder, DepartmentSeeder>();
            services.AddScoped<ISeeder, EmployeeSeeder>();
            services.AddScoped<ISeeder, CustomerSeeder>();
            services.AddScoped<ISeeder, SupplierSeeder>();

            return services;

        }
    }
}
