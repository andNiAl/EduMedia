using EduMedia.Web.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =============================================================
// 🔹 CONFIGURAR SERVICIOS
// =============================================================

// 🔹 Conexión a PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Controladores con vistas (MVC)
builder.Services.AddControllersWithViews();

// 🔹 Habilitar acceso al contexto HTTP (necesario para login, sesión, etc.)
builder.Services.AddHttpContextAccessor();

// 🔹 Configurar sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // tiempo de inactividad permitido
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =============================================================
// 🔹 CONSTRUIR APP
// =============================================================
var app = builder.Build();

// =============================================================
// 🔹 APLICAR MIGRACIONES AUTOMÁTICAS AL INICIAR (opcional)
// =============================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// =============================================================
// 🔹 CONFIGURAR PIPELINE HTTP
// =============================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔹 Activar uso de sesión
app.UseSession();

// 🔹 Si más adelante agregas autenticación, esto debe ir antes de Authorization:
// app.UseAuthentication();

app.UseAuthorization();

// =============================================================
// 🔹 CONFIGURAR RUTAS
// =============================================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// =============================================================
// 🔹 EJECUTAR APP
// =============================================================
app.Run();
