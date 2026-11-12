namespace ERP.Application.Common.Caching;

public static class CacheKeys
{
    public static class Catalog
    {
        public static string ProductById(int id) => $"Catalog:Product:{id}";
        public static string ProductsPrefix() => "Catalog:Products";
        public static string ProductsList(string searchPart, string categoryPart, int page, int pageSize)
            => $"{ProductsPrefix()}:Search={searchPart}:Category={categoryPart}:Page={page}:Size={pageSize}";
        public static string CategoryById(int id) => $"Catalog:Category:{id}";
        public static string CategoriesPrefix() => "Catalog:Categories";
    }

    public static class Sales
    {
        public static string OrderById(int id) => $"Sales:Order:{id}";
        public static string OrdersPrefix() => "Sales:Orders";
        public static string OrdersList(string custPart, string statPart, string searchPart, int page, int pageSize)
            => $"{OrdersPrefix()}:Cust={custPart}:Stat={statPart}:Search={searchPart}:Page={page}:Size={pageSize}";
        public static string CustomerById(int id) => $"Sales:Customer:{id}";
        public static string CustomersPrefix() => "Sales:Customers";
        public static string CustomersList(string searchPart, int page, int pageSize)
            => $"{CustomersPrefix()}:Search={searchPart}:Page={page}:Size={pageSize}";
    }

    public static class Purchasing
    {
        public static string SupplierById(int id) => $"Purchasing:Supplier:{id}";
        public static string SuppliersPrefix() => "Purchasing:Suppliers";
        public static string SuppliersList(string searchPart, int page, int pageSize)
            => $"{SuppliersPrefix()}:Search={searchPart}:Page={page}:Size={pageSize}";
        public static string PurchaseOrderById(int id) => $"PO:PurchaseOrder:{id}";
        public static string PurchaseOrdersPrefix() => "PO:PurchaseOrders";
        public static string PurchaseOrdersList(string supPart, string statPart, string searchPart, int page, int pageSize)
            => $"{PurchaseOrdersPrefix()}:Sup={supPart}:Stat={statPart}:Search={searchPart}:Page={page}:Size={pageSize}";
    }

    public static class Finance
    {
        public static string InvoiceById(int id) => $"Finance:Invoice:{id}";
        public static string InvoicesPrefix() => "Finance:Invoices";
        public static string InvoicesList(string custPart, string statPart, string searchPart, int page, int pageSize)
            => $"{InvoicesPrefix()}:Cust={custPart}:Stat={statPart}:Search={searchPart}:Page={page}:Size={pageSize}";
    }

    public static class HR
    {
        public static string EmployeeById(int id) => $"HR:Employee:{id}";
        public static string EmployeesPrefix() => "HR:Employees";
        public static string EmployeesList(string depPart, string searchPart, int page, int pageSize)
            => $"{EmployeesPrefix()}:Dep={depPart}:Search={searchPart}:Page={page}:Size={pageSize}";
        public static string DepartmentsPrefix() => "HR:Departments";
    }
}
