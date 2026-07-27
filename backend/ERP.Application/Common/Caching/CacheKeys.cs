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
}
