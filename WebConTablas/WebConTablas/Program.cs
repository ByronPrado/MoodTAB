using Microsoft.EntityFrameworkCore;
using WebConTablas.Controllers; // 👈 Aquí está tu ChatService

var builder = WebApplication.CreateBuilder(args);

// Agregar contexto con conexión PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

builder.Services.AddSession();

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// 👇 REGISTRO DEL CHATBOT SERVICE
builder.Services.AddSingleton<ChatService>(sp =>
{
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    if (string.IsNullOrEmpty(apiKey))
        throw new Exception("❌ No se encontró la API Key en OPENAI_API_KEY.");

    return new ChatService(apiKey);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseSession();

app.UseStaticFiles();

app.UseRouting();

app.UseCors();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Psiquiatras}/{action=Login}/{id?}");

app.Run();
