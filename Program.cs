using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using USMPWEB.Data;

var builder = WebApplication.CreateBuilder(args);

// Configura la conexión a la base de datos
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection") 
    ?? throw new InvalidOperationException("Connection string 'PostgreSQLConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Agrega servicios para la identidad
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false; // Cambia a true si necesitas confirmación de correo
    options.Password.RequireDigit = true; // Configura la política de contraseñas según sea necesario
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configuración de autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index"; // Ruta para redirigir cuando el usuario no está autenticado
        options.LogoutPath = "/Login/Logout"; // Ruta para cerrar sesión
    });

// Agrega soporte para controladores de API y vistas
builder.Services.AddControllers(); // Permitir solo controladores de API
builder.Services.AddControllersWithViews(); // Para controladores MVC

var app = builder.Build();

// Configura la tubería de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Usa autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Rutas de controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapea rutas para controladores de API
app.MapControllers(); // Permitir el uso de controladores API

app.MapRazorPages();

app.Run();
