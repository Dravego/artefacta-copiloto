using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Artefacta.Copiloto.Security;
using Artefacta.Copiloto.Models;
using Artefacta.Copiloto.Data;
using Artefacta.Copiloto.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UsuarioActual>();
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddScoped<AutenticacionService>();
builder.Services.AddScoped<AdministracionUsuariosService>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        options.Cookie.Name = "Artefacta.Copiloto.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();


var connectionString = builder.Configuration.GetConnectionString("OracleConnection")
    ?? throw new InvalidOperationException("No se encontró ConnectionStrings:OracleConnection.");

builder.Services.AddDbContext<ArtefactaDbContext>(options =>
    options.UseOracle(connectionString));

builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<VentaService>();
builder.Services.AddScoped<OportunidadService>();
builder.Services.AddScoped<RecomendacionService>();
builder.Services.AddScoped<CopilotoService>();
builder.Services.AddScoped<CopilotoConversacionalService>();
builder.Services.AddScoped<CompraService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
