
namespace ERP.Infrastructure.Persistence
{
    using System.Linq.Expressions;

    using ERP.Application.Abstractions.Common;
    using ERP.Domain.Catalog.Categories;
    using ERP.Domain.Catalog.Products;
    using ERP.Domain.HR.Departments;
    using ERP.Domain.HR.Employees;
    using ERP.Domain.Identity.Roles;
    using ERP.Domain.Identity.Users;
    using ERP.Domain.Purchasing.PurchaseOrders;
    using ERP.Domain.Purchasing.Suppliers;
    using ERP.Domain.Sales.Customers;
    using ERP.Domain.Sales.Invoices;
    using ERP.Domain.Sales.Orders;
    using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Abstractions;

    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService? _currentUserService;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentUserService? currentUserService = null) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var filter = Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted)),
                            Expression.Constant(false)),
                        parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }

        private void OnBeforeSaving()
        {
            var currentUserId = _currentUserService?.UserId ?? _currentUserService?.UserName;
            var now = DateTimeOffset.UtcNow;

            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.CreatedBy = currentUserId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = now;
                        entry.Entity.LastModifiedBy = currentUserId;
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = now;
                    entry.Entity.DeletedBy = currentUserId;
                }
            }
        }
    }
}
