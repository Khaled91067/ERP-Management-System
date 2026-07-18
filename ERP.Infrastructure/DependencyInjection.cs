using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Authentication;
using ERP.Application.Abstractions.Repositories;
using ERP.Infrastructure.Authentication;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Persistence.Repositories;
using ERP.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace ERP.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options
                    .UseSqlServer(configuration
                    .GetConnectionString("DefaultConnection")));

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
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));



            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();


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





            return services;

        }
    }
}
