using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Anda.ServiciosAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregamos servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<ServicioIAClient>();
var AllowFrontendOrigins = "_AllowFrontendOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowFrontendOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173", "front-production-1fc1.up.railway.app", "front", "https://front-production-1fc1.up.railway.app")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// 2. Configuramos EF Core con SQLite (anda.db en la raíz)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=anda.db"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(AllowFrontendOrigins);

app.UseAuthorization();
app.MapControllers();

// Log del puerto de ejecución
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000"; // Usa el puerto especificado en el entorno o por defecto 5000
Console.WriteLine($"Aplicación corriendo en el puerto {port}");

// Configura el puerto
app.Run($"http://0.0.0.0:{port}");
