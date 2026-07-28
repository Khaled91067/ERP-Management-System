using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHasDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {














            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Employees",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedAt",
                table: "Employees",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Departments",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedAt",
                table: "Departments",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Categories",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedAt",
                table: "Categories",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Categories");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DeletedAt", "DeletedBy", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, null, null, false, "Electronics" },
                    { 2, null, null, false, "Office Supplies" },
                    { 3, null, null, false, "Industrial Equipment" },
                    { 4, null, null, false, "Packaging" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Address", "City", "Country", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Email", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "Name", "Phone", "TaxId" },
                values: new object[,]
                {
                    { 1, "1200 Market Street, Suite 800", "New York", "USA", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "purchasing@alnoortrading.com", false, null, null, "Al Noor Trading Co.", "+1-212-555-0148", "US-TAX-100245" },
                    { 2, "44 King William Street", "London", "UK", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "accounts@bluewaveretail.com", false, null, null, "BlueWave Retail Ltd.", "+44-20-5550-2211", "GB-TAX-778845" },
                    { 3, "Business Bay Tower 18", "Dubai", "UAE", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "billing@horizonconstruction.com", false, null, null, "Horizon Construction LLC", "+971-4-555-0198", "AE-TAX-442110" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "DeletedAt", "DeletedBy", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, null, null, false, "Sales" },
                    { 2, null, null, false, "Operations" },
                    { 3, null, null, false, "Finance" },
                    { 4, null, null, false, "Procurement" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name", "Permissions" },
                values: new object[,]
                {
                    { 1, "Administrator", "[\"Users.Read\",\"Users.Write\",\"Orders.Read\",\"Orders.Write\",\"Reports.Read\",\"Reports.Write\"]" },
                    { 2, "Sales Manager", "[\"Customers.Read\",\"Customers.Write\",\"Orders.Read\",\"Orders.Write\",\"Invoices.Read\"]" },
                    { 3, "Warehouse Clerk", "[\"Products.Read\",\"PurchaseOrders.Read\",\"PurchaseOrders.Write\",\"Inventory.Update\"]" }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "CompanyName", "ContactName", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Email", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "PaymentTerms", "Phone" },
                values: new object[,]
                {
                    { 1, "Global Tech Distributors", "Maya Peterson", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "sales@globaltechdist.com", false, null, null, "Net 30", "+1-646-555-0133" },
                    { 2, "Metro Office Supply", "Daniel Cooper", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "orders@metroofficesupply.com", false, null, null, "Net 15", "+44-20-5550-3344" },
                    { 3, "Prime Industrial Parts", "Nadia Rahman", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, "procurement@primeindustrialparts.com", false, null, null, "Due on receipt", "+971-4-555-0220" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DeletedAt", "DeletedBy", "DepartmentId", "Email", "FirstName", "HireDate", "IsDeleted", "LastName", "Phone", "Position", "Salary" },
                values: new object[,]
                {
                    { 1, null, null, 1, "layla.hassan@erpco.com", "Layla", new DateTime(2023, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Hassan", "+1-212-555-0101", "Sales Executive", 14500m },
                    { 2, null, null, 2, "omar.farouk@erpco.com", "Omar", new DateTime(2022, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Farouk", "+1-212-555-0102", "Operations Supervisor", 16800m },
                    { 3, null, null, 3, "rania.khaled@erpco.com", "Rania", new DateTime(2021, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Khaled", "+1-212-555-0103", "Finance Specialist", 15500m },
                    { 4, null, null, 4, "tarek.youssef@erpco.com", "Tarek", new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Youssef", "+1-212-555-0104", "Procurement Officer", 13200m }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CustomerId", "DeletedAt", "DeletedBy", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "OrderDate", "PaymentMethod", "ShippingAddress", "Status", "TotalAmount" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", 1, null, null, false, null, null, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "CreditCard", "1200 Market Street, Suite 800, New York, NY", 2, 2004m },
                    { 2, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", 2, null, null, false, null, null, new DateTime(2025, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cash", "44 King William Street, London", 3, 1100m },
                    { 3, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", 3, null, null, false, null, null, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "MobilePayment", "Business Bay Tower 18, Dubai", 1, 470m }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CostPrice", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "Name", "ReorderLevel", "Sku", "StockQuantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 2, 620m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, false, null, null, "Laser Printer", 5, "PRN-LP850", 18, 850m },
                    { 2, 2, 145m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, false, null, null, "Ergonomic Office Chair", 10, "CHR-OC240", 30, 240m },
                    { 3, 1, 210m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, false, null, null, "Wireless Barcode Scanner", 6, "SCN-WBS320", 14, 320m },
                    { 4, 3, 110m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, false, null, null, "Industrial Sensor", 12, "SNS-IS180", 40, 180m },
                    { 5, 4, 18m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, false, null, null, "Heavy Duty Carton Box Pack", 50, "BOX-HD035", 150, 35m },
                    { 6, 3, 30m, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, false, null, null, "Safety Helmet", 20, "SFT-HM055", 80, 55m }
                });

            migrationBuilder.InsertData(
                table: "PurchaseOrders",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "ExpectedDelivery", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "OrderDate", "Status", "SupplierId", "TotalAmount" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, new DateTime(2025, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Received", 1, 5200m },
                    { 2, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Approved", 2, 1880m },
                    { 3, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", null, null, new DateTime(2025, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Draft", 3, 1700m }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "PasswordHash", "RoleId" },
                values: new object[,]
                {
                    { 1, "omar.saleh@erpco.com", "Omar", "Saleh", "$2a$11$k5X4wQb3G4z7kQv8Ff9M2O1hQ2rM5g7uN6p7v8w9x0y1z2a3b4c5d", 1 },
                    { 2, "sara.ibrahim@erpco.com", "Sara", "Ibrahim", "$2a$11$g2H7nQp4R8s1tV6wX9y0zAaBbCcDdEeFfGgHhIiJjKkLlMmNnOoP", 2 },
                    { 3, "ahmed.nasser@erpco.com", "Ahmed", "Nasser", "$2a$11$7uY2dK9mS1pQ4rT6vW8xZ0AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoQ", 3 }
                });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CustomerId", "DeletedAt", "DeletedBy", "DueDate", "InvoiceDate", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "OrderId", "PaidAt", "Status", "TotalAmount" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", 1, null, null, new DateTime(2025, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, 1, null, "Sent", 2004m },
                    { 2, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", 2, null, null, new DateTime(2025, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, 2, new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Paid", 1100m },
                    { 3, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", 3, null, null, new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, 3, null, "Draft", 470m }
                });

            migrationBuilder.InsertData(
                table: "OrderLines",
                columns: new[] { "Id", "DiscountPercentage", "OrderId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 0m, 1, 1, 2, 850m },
                    { 2, 5m, 1, 3, 1, 320m },
                    { 3, 0m, 2, 2, 4, 240m },
                    { 4, 0m, 2, 5, 4, 35m },
                    { 5, 0m, 3, 4, 2, 180m },
                    { 6, 0m, 3, 6, 2, 55m }
                });

            migrationBuilder.InsertData(
                table: "PurchaseOrderLines",
                columns: new[] { "Id", "ProductId", "PurchaseOrderId", "Quantity", "UnitCost" },
                values: new object[,]
                {
                    { 1, 1, 1, 5, 620m },
                    { 2, 3, 1, 10, 210m },
                    { 3, 2, 2, 8, 145m },
                    { 4, 5, 2, 40, 18m },
                    { 5, 4, 3, 10, 110m },
                    { 6, 6, 3, 20, 30m }
                });

            migrationBuilder.InsertData(
                table: "InvoiceLines",
                columns: new[] { "Id", "Description", "InvoiceId", "Quantity", "TaxRate", "UnitPrice" },
                values: new object[,]
                {
                    { 1, "Laser Printer - Model LP850", 1, 2, 5m, 850m },
                    { 2, "Wireless Barcode Scanner - Model WBS320", 1, 1, 5m, 320m },
                    { 3, "Ergonomic Office Chair - Model OC240", 2, 4, 0m, 240m },
                    { 4, "Heavy Duty Carton Box Pack", 2, 4, 0m, 35m },
                    { 5, "Industrial Sensor - Model IS180", 3, 2, 15m, 180m }
                });
        }
    }
}

