namespace ERP.Application.Common.Caching;

public static class CacheKeys
{
    public static class Products
    {
        public static string ById(int id) => $"Products-{id}";
        public static string ListPrefix() => "ProductsList-";
        public static string List(int page, int pageSize, string search, string sortBy, string sortDir) 
            => $"{ListPrefix()}{page}-{pageSize}-{search}-{sortBy}-{sortDir}";
    }

    public static class Categories
    {
        public static string All() => "Categories-All";
        public static string ById(int id) => $"Categories-{id}";
    }

    public static class Dashboard
    {
        public static string Summary() => "Dashboard-Summary";
    }

    public static class Users
    {
        public static string ById(int id) => $"Users-{id}";
    }

    public static class Sales
    {
        public static string OrderById(int id) => $"Sales:Order:{id}";
        public static string OrdersPrefix() => "Sales:Orders";
        public static string OrdersList(string custPart, string statPart, string searchPart, int page, int pageSize) 
            => $"{OrdersPrefix()}:Cust={custPart}:Stat={statPart}:Search={searchPart}:Page={page}:Size={pageSize}";
        public static string CustomerById(int id) => $"Sales:Customer:{id}";
        public static string CustomersPrefix() => "Sales:Customers";
    }

    public static class Purchasing
    {
        public static string SupplierById(int id) => $"Purchasing:Supplier:{id}";
        public static string SuppliersPrefix() => "Purchasing:Suppliers";
        public static string SuppliersList(string searchPart, int page, int pageSize) 
            => $"{SuppliersPrefix()}:Search={searchPart}:Page={page}:Size={pageSize}";
    }
}
