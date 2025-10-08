using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using TelegramBotApi.Data;
using TelegramBotApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:" + (Environment.GetEnvironmentVariable("PORT") ?? "8080"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",                         
                "https://localhost:5173",                        
                "https://effervescent-malabi-475f7f.netlify.app", 
                "https://*.netlify.app",                         
                "https://web.telegram.org",                      
                "https://oauth.telegram.org"                     
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Добавляем контроллеры
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                      ?? "Data Source=tmk.db";

builder.Services.AddDbContext<TMKDbContext>(options =>
    options.UseSqlite(connectionString));

// Регистрируем сервисы
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<PriceCalculatorService>();

// Добавляем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TMK MiniApp API", Version = "v1" });
});

var app = builder.Build();

app.UseRouting(); 
app.UseCors(); 

// Инициализация БД
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TMKDbContext>();
    await db.Database.EnsureCreatedAsync();
    
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TMK MiniApp API v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();
app.MapControllers();

app.Run();