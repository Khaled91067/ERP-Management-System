namespace ERP.Domain.Identity.Permissions;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ERP.Domain.Identity.Roles;

public static class Permissions
{
    public static class Customers
    {
        public const string Read = "Customers.Read";
        public const string Write = "Customers.Write";
    }

    public static class Orders
    {
        public const string Read = "Orders.Read";
        public const string Write = "Orders.Write";
    }

    public static class Products
    {
        public const string Read = "Products.Read";
        public const string Write = "Products.Write";
        public const string AdjustStock = "Products.AdjustStock";
    }

    public static class Categories
    {
        public const string Read = "Categories.Read";
        public const string Write = "Categories.Write";
    }

    public static class Suppliers
    {
        public const string Read = "Suppliers.Read";
        public const string Write = "Suppliers.Write";
    }

    public static class PurchaseOrders
    {
        public const string Read = "PurchaseOrders.Read";
        public const string Write = "PurchaseOrders.Write";
    }

    public static class Invoices
    {
        public const string Read = "Invoices.Read";
        public const string Write = "Invoices.Write";
        public const string Send = "Invoices.Send";
        public const string Pay = "Invoices.Pay";
    }

    public static class Employees
    {
        public const string Read = "Employees.Read";
        public const string Write = "Employees.Write";
    }

    public static class Departments
    {
        public const string Read = "Departments.Read";
        public const string Write = "Departments.Write";
    }

    public static class Users
    {
        public const string Read = "Users.Read";
        public const string Write = "Users.Write";
    }

    public static class Roles
    {
        public const string Read = "Roles.Read";
        public const string Write = "Roles.Write";
    }

    public static readonly IReadOnlyList<string> All = typeof(Permissions)
        .GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
        .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => (string)f.GetValue(null)!))
        .Distinct()
        .ToList()
        .AsReadOnly();

    public static IReadOnlyList<string> GetDefaultPermissions(string roleName) => roleName switch
    {
        RoleConstants.Administrator => All,

        RoleConstants.Sales =>
        [
            Customers.Read,
            Customers.Write,
            Orders.Read,
            Orders.Write,
            Products.Read,
            Categories.Read,
            Invoices.Read
        ],

        RoleConstants.Purchasing =>
        [
            Suppliers.Read,
            Suppliers.Write,
            PurchaseOrders.Read,
            PurchaseOrders.Write,
            Products.Read,
            Categories.Read
        ],

        RoleConstants.Warehouse =>
        [
            Products.Read,
            Products.Write,
            Products.AdjustStock,
            Categories.Read,
            Categories.Write
        ],

        RoleConstants.Finance =>
        [
            Customers.Read,
            Invoices.Read,
            Invoices.Write,
            Invoices.Send,
            Invoices.Pay
        ],

        RoleConstants.HR =>
        [
            Employees.Read,
            Employees.Write,
            Departments.Read,
            Departments.Write
        ],

        _ => []
    };
}
