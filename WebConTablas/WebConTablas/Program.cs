using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Opcional: configurar reglas de contraseña, bloqueo, etc.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Psiquiatras/Login";    // Página de login
    options.AccessDeniedPath = "/Psiquiatras/AccessDenied"; // Página para acceso denegado (opcional)
});

// Servicios personalizados
builder.Services.AddScoped<IFormularioService, FormularioService>();
builder.Services.AddScoped<IPsiquiatraService, PsiquiatraService>();
builder.Services.AddScoped<IFormularioAsignadoService, FormularioAsignadoService>();
builder.Services.AddScoped<IFormularioPreguntaService, FormularioPreguntaService>();
builder.Services.AddScoped<IVistaDatosService, VistaDatosService>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IPreguntaService, PreguntaService>();
builder.Services.AddScoped<ISesionService, SesionService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();

builder.Services.AddSession();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseCors();

app.UseSession();

// ** IMPORTANTE: Agregar autenticación y autorización **
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Psiquiatras}/{action=Login}/{id?}");

app.Run();
