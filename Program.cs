using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using TelegramBotApi.Data;
using TelegramBotApi.Services; // ✅ Добавлена

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры
builder.Services.AddControllers();

// ✅ Подключаем контекст БД
builder.Services.AddDbContext<TMKDbContext>(options =>
    options.UseSqlite("Data Source=tmk.db"));

// ✅ Регистрируем сервисы
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<PriceCalculatorService>();

// Добавляем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TMK MiniApp API", Version = "v1" });
});

var app = builder.Build();

// ✅ Инициализация БД
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TMKDbContext>();
    await db.Database.EnsureCreatedAsync();
    
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TMK MiniApp API v1");
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();