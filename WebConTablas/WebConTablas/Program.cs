using Microsoft.EntityFrameworkCore;
using WebConTablas.Controllers;  // 👈 1. Asegúrate de tener este using para AnalysisSettings
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration; // Necesario para Configuration
using System; // Necesario para Environment y Exception
using WebConTablas.Models; // Asumo que AppDbContext y Models están aquí
using WebConTablas.Services;
using WebConTablas.Common; // <-- Agrega este using

var builder = WebApplication.CreateBuilder(args);

// --- Configuración de Servicios ---

builder.Services.AddScoped<EmotionalPredictionService>();

// Agregar contexto con conexión PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddControllers();

// 1. Configuración del mapeo de opciones
builder.Services.Configure<WebConTablas.Common.AnalysisSettings>(
    builder.Configuration.GetSection("AnalysisSettings"));

// 2. Registro de servicios dependientes
// Estos servicios necesitan IOptions<AnalysisSettings> en su constructor
builder.Services.AddSingleton<SensitiveWordDetector>();
builder.Services.AddSingleton<MessageAnalyzer>();

// 3. REGISTRO ÚNICO Y CORRECTO DEL CHATSERVICE
builder.Services.AddSingleton<ChatService>(provider =>
{
    // Obtener la API Key de la configuración (appsettings.json o environment)
    // Usamos el acceso por índice que funciona bien para appsettings.json
    var apiKey = builder.Configuration["OpenAI:ApiKey"];
    
    if (string.IsNullOrEmpty(apiKey))
        throw new Exception("❌ No se encontró la API Key en la configuración (OpenAI:ApiKey).");

    // Obtener los servicios MessageAnalyzer y SensitiveWordDetector
    var analyzer = provider.GetRequiredService<MessageAnalyzer>();
    var detector = provider.GetRequiredService<SensitiveWordDetector>();

    // Construir ChatService con todos los argumentos requeridos.
    return new ChatService(apiKey, analyzer, detector); 
});
// ❌ Eliminado el registro duplicado: builder.Services.AddSingleton<ChatService>(sp => { ... });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- Construcción y Middleware ---

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseSession();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();

// Mapeo de controladores API
app.MapControllers();

// Mapeo de ruta por defecto (para vistas MVC)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Psiquiatras}/{action=Login}/{id?}");

app.Run();