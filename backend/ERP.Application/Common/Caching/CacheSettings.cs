namespace ERP.Application.Common.Caching;

public class CacheSettings
{
    public int ReferenceDataExpirationMinutes { get; set; } = 60;
    public int FrequentDataExpirationMinutes { get; set; } = 15;
    public int PaginatedListExpirationMinutes { get; set; } = 5;
}
