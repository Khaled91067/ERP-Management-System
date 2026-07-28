namespace ERP.Infrastructure.Persistence.Seeding;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public static class SeedHelper
{
    public static async Task<List<T>> ReadSeedDataAsync<T>(string fileName)
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Persistence", "SeedData", fileName);
        if (!File.Exists(path)) return new List<T>();

        var json = await File.ReadAllTextAsync(path);
        var options = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        return JsonSerializer.Deserialize<List<T>>(json, options) ?? new List<T>();
    }
}
