using EventosCostaRica.Datos;
using EventosCostaRica.Negocio;
using EventosCostaRica.Web.Infraestructura;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Toda la aplicación exige inicio de sesión, salvo lo marcado con [AllowAnonymous].
builder.Services.AddControllersWithViews(opciones =>
{
    var politica = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opciones.Filters.Add(new AuthorizeFilter(politica));
});


// Autenticación con cookies y autorización por roles.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.LoginPath = "/Cuenta/Login";
        opciones.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        opciones.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization();

// Base de datos y repositorio genérico.
builder.Services.AddDbContext<AppDbContext>(opciones =>
    opciones.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

// Servicios de negocio.
builder.Services.AddSingleton<IHasheadorContrasena, HasheadorContrasena>();
builder.Services.AddScoped<IUsuarioServicio, UsuarioServicio>();
builder.Services.AddScoped<IEventoServicio, EventoServicio>();
builder.Services.AddScoped<IEntradaServicio, EntradaServicio>();

// Manejador global de excepciones.
builder.Services.AddExceptionHandler<ManejadorGlobalExcepciones>();

var app = builder.Build();

// Crea la base de datos SQLite y carga los datos de ejemplo la primera vez.
using (var alcance = app.Services.CreateScope())
{
    alcance.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
    await InicializadorBaseDatos.SembrarAsync(alcance.ServiceProvider);
}

app.UseExceptionHandler("/Error");
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
