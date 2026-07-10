using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Electronics" },
                    { 2, "Office Supplies" },
                    { 3, "Industrial Equipment" },
                    { 4, "Packaging" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Address", "City", "Country", "Email", "Name", "Phone", "TaxId" },
                values: new object[,]
                {
                    { 1, "1200 Market Street, Suite 800", "New York", "USA", "purchasing@alnoortrading.com", "Al Noor Trading Co.", "+1-212-555-0148", "US-TAX-100245" },
                    { 2, "44 King William Street", "London", "UK", "accounts@bluewaveretail.com", "BlueWave Retail Ltd.", "+44-20-5550-2211", "GB-TAX-778845" },
                    { 3, "Business Bay Tower 18", "Dubai", "UAE", "billing@horizonconstruction.com", "Horizon Construction LLC", "+971-4-555-0198", "AE-TAX-442110" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Sales" },
                    { 2, "Operations" },
                    { 3, "Finance" },
                    { 4, "Procurement" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "Permissions" },
                values: new object[] { "Administrator", "[\"Users.Read\",\"Users.Write\",\"Orders.Read\",\"Orders.Write\",\"Reports.Read\",\"Reports.Write\"]" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "Permissions" },
                values: new object[] { "Sales Manager", "[\"Customers.Read\",\"Customers.Write\",\"Orders.Read\",\"Orders.Write\",\"Invoices.Read\"]" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name", "Permissions" },
                values: new object[] { 3, "Warehouse Clerk", "[\"Products.Read\",\"PurchaseOrders.Read\",\"PurchaseOrders.Write\",\"Inventory.Update\"]" });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "CompanyName", "ContactName", "Email", "PaymentTerms", "Phone" },
                values: new object[,]
                {
                    { 1, "Global Tech Distributors", "Maya Peterson", "sales@globaltechdist.com", "Net 30", "+1-646-555-0133" },
                    { 2, "Metro Office Supply", "Daniel Cooper", "orders@metroofficesupply.com", "Net 15", "+44-20-5550-3344" },
                    { 3, "Prime Industrial Parts", "Nadia Rahman", "procurement@primeindustrialparts.com", "Due on receipt", "+971-4-555-0220" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "PasswordHash", "RoleId" },
                values: new object[,]
                {
                    { 1, "omar.saleh@erpco.com", "Omar", "Saleh", "$2a$11$k5X4wQb3G4z7kQv8Ff9M2O1hQ2rM5g7uN6p7v8w9x0y1z2a3b4c5d", 1 },
                    { 2, "sara.ibrahim@erpco.com", "Sara", "Ibrahim", "$2a$11$g2H7nQp4R8s1tV6wX9y0zAaBbCcDdEeFfGgHhIiJjKkLlMmNnOoP", 2 }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DepartmentId", "Email", "FirstName", "HireDate", "LastName", "Phone", "Position", "Salary" },
                values: new object[,]
                {
                    { 1, 1, "layla.hassan@erpco.com", "Layla", new DateTime(2023, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hassan", "+1-212-555-0101", "Sales Executive", 14500m },
                    { 2, 2, "omar.farouk@erpco.com", "Omar", new DateTime(2022, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Farouk", "+1-212-555-0102", "Operations Supervisor", 16800m },
                    { 3, 3, "rania.khaled@erpco.com", "Rania", new DateTime(2021, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khaled", "+1-212-555-0103", "Finance Specialist", 15500m },
                    { 4, 4, "tarek.youssef@erpco.com", "Tarek", new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Youssef", "+1-212-555-0104", "Procurement Officer", 13200m }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CustomerId", "OrderDate", "PaymentMethod", "ShippingAddress", "Status", "TotalAmount" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "CreditCard", "1200 Market Street, Suite 800, New York, NY", 2, 2004m },
                    { 2, 2, new DateTime(2025, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cash", "44 King William Street, London", 3, 1100m },
                    { 3, 3, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "MobilePayment", "Business Bay Tower 18, Dubai", 1, 470m }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CostPrice", "Name", "ReorderLevel", "Sku", "StockQuantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 2, 620m, "Laser Printer", 5, "PRN-LP850", 18, 850m },
                    { 2, 2, 145m, "Ergonomic Office Chair", 10, "CHR-OC240", 30, 240m },
                    { 3, 1, 210m, "Wireless Barcode Scanner", 6, "SCN-WBS320", 14, 320m },
                    { 4, 3, 110m, "Industrial Sensor", 12, "SNS-IS180", 40, 180m },
                    { 5, 4, 18m, "Heavy Duty Carton Box Pack", 50, "BOX-HD035", 150, 35m },
                    { 6, 3, 30m, "Safety Helmet", 20, "SFT-HM055", 80, 55m }
                });

            migrationBuilder.InsertData(
                table: "PurchaseOrders",
                columns: new[] { "Id", "ExpectedDelivery", "OrderDate", "Status", "SupplierId", "TotalAmount" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Received", 1, 5200m },
                    { 2, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Approved", 2, 1880m },
                    { 3, new DateTime(2025, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Draft", 3, 1700m }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "PasswordHash", "RoleId" },
                values: new object[] { 3, "ahmed.nasser@erpco.com", "Ahmed", "Nasser", "$2a$11$7uY2dK9mS1pQ4rT6vW8xZ0AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoQ", 3 });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "Id", "CustomerId", "DueDate", "InvoiceDate", "OrderId", "PaidAt", "Status", "TotalAmount" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, "Sent", 2004m },
                    { 2, 2, new DateTime(2025, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Paid", 1100m },
                    { 3, 3, new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, null, "Draft", 470m }
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
                table: "PurchaseLines",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "InvoiceLines",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "InvoiceLines",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "InvoiceLines",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "InvoiceLines",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "InvoiceLines",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderLines",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderLines",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderLines",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderLines",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OrderLines",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderLines",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "PurchaseLines",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PurchaseLines",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PurchaseLines",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PurchaseLines",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PurchaseLines",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "PurchaseLines",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "PurchaseOrders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PurchaseOrders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PurchaseOrders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "Permissions" },
                values: new object[] { "Admin", "[]" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "Permissions" },
                values: new object[] { "User", "[]" });
        }
    }
}
