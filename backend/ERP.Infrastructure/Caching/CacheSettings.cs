namespace ERP.Infrastructure.Caching;

public class CacheSettings
{
    public int ProductsTtlMinutes { get; set; } = 5;
    public int CategoriesTtlMinutes { get; set; } = 30;
    public int DashboardTtlMinutes { get; set; } = 1;
    public int DefaultTtlMinutes { get; set; } = 720; // 12 hours
}
