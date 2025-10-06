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
            if (_db.Nomenclatures.Any()) return;

            var basePath = Path.Combine(_env.ContentRootPath, "DataFiles");

            // ✅ ОТКЛЮЧАЕМ FOREIGN KEYS ДЛЯ ВСЕЙ СЕССИИ
            await _db.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = OFF;");

            try
            {
                // Загружаем в правильном порядке
                await SeedFromJson<Stock>(_db.Stocks, Path.Combine(basePath, "stocks.json"), "ArrayOfStockEl");
                await SeedFromJson<Nomenclature>(_db.Nomenclatures, Path.Combine(basePath, "nomenclature.json"), "ArrayOfNomenclatureEl");
                await SeedFromJson<Price>(_db.Prices, Path.Combine(basePath, "prices.json"), "ArrayOfPricesEl");
                await SeedFromJson<Remnant>(_db.Remnants, Path.Combine(basePath, "remnants.json"), "ArrayOfRemnantsEl");
            }
            finally
            {
                // ✅ ВКЛЮЧАЕМ ОБРАТНО
                await _db.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;");
            }
        }

        private async Task SeedFromJson<T>(DbSet<T> dbSet, string path, string rootName) where T : class
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"File not found: {path}");
                return;
            }

            try
            {
                using var stream = File.OpenRead(path);
                using var doc = await JsonDocument.ParseAsync(stream);

                var root = doc.RootElement.GetProperty(rootName);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                var items = JsonSerializer.Deserialize<List<T>>(root.GetRawText(), options);

                if (items != null && items.Any())
                {
                    Console.WriteLine($"Loading {items.Count} items from {path}");
                    await dbSet.AddRangeAsync(items);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {path}: {ex.Message}");
            }
        }
    }
}