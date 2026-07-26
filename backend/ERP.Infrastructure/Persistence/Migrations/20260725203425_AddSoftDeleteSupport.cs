using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Sku",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Departments_Name",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Suppliers",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Suppliers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "PurchaseOrders",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "PurchaseOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PurchaseOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Products",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Orders",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Invoices",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Invoices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Employees",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Departments",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Departments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Customers",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Categories",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "PurchaseOrders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "PurchaseOrders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "PurchaseOrders",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { null, null, false });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                table: "Products",
                column: "Sku",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Sku",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Departments_Name",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                table: "Products",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);
        }
    }
}
