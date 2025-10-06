using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using TelegramBotApi.Data;
using TelegramBotApi.Entities;

namespace TelegramBotApi.Services
{
    public class DataSeeder
    {
        private readonly TMKDbContext _db;
        private readonly IWebHostEnvironment _env;

        public DataSeeder(TMKDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task SeedAsync()
        {
            if (_db.Nomenclatures.Any()) return; // уже загружено

            var basePath = Path.Combine(_env.ContentRootPath, "DataFiles");

            await SeedFromJson<Nomenclature>(_db.Nomenclatures, Path.Combine(basePath, "nomenclature.json"), "ArrayOfNomenclatureEl");
            await SeedFromJson<Price>(_db.Prices, Path.Combine(basePath, "prices.json"), "ArrayOfPricesEl");
            await SeedFromJson<Remnant>(_db.Remnants, Path.Combine(basePath, "remnants.json"), "ArrayOfRemnantsEl");
            await SeedFromJson<Stock>(_db.Stocks, Path.Combine(basePath, "stocks.json"), "ArrayOfStockEl");
        }

        private async Task SeedFromJson<T>(DbSet<T> dbSet, string path, string rootName) where T : class
{
    if (!File.Exists(path)) return;

    using var stream = File.OpenRead(path);
    using var doc = await JsonDocument.ParseAsync(stream);

    var root = doc.RootElement.GetProperty(rootName);

    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new EmptyStringConverter() }
    };

    var items = JsonSerializer.Deserialize<List<T>>(root.GetRawText(), options);

    if (items != null && items.Any())
    {
        await dbSet.AddRangeAsync(items);
        await _db.SaveChangesAsync();
    }
}

public class EmptyStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType == JsonTokenType.Null ? string.Empty : reader.GetString() ?? string.Empty;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
    }
}
