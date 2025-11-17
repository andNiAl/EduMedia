using EduMedia.Web.Data;
using EduMedia.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace EduMedia.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 Página de registro
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // 🔹 Crear nuevo usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe el correo
                bool existe = await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo);
                if (existe)
                {
                    ModelState.AddModelError("", "El correo ya está registrado.");
                    return View(usuario);
                }

                // 🔹 Asignar valores por defecto
                usuario.Rol = "Usuario";
                usuario.FechaRegistro = DateTime.UtcNow; // ✅ Evita errores de timestamp
                usuario.Contrasena = HashPassword(usuario.Contrasena); // 🔒 Hash de la contraseña

                // 🔹 Guardar en BD
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Cuenta creada correctamente. Inicia sesión.";
                return RedirectToAction("Login");
            }

            return View(usuario);
        }

        // 🔹 Página de login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 🔹 Procesar login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                ViewBag.Error = "Por favor, ingresa tus credenciales.";
                return View();
            }

            // 🔒 Generar hash y comparar
            string hashed = HashPassword(contrasena);
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo && u.Contrasena == hashed);

            if (usuario == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View();
            }

            // 🔹 Guardar sesión
            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
            HttpContext.Session.SetString("UsuarioRol", usuario.Rol);

            // ✅ Redirigir a ContentItems tras iniciar sesión
            return RedirectToAction("Index", "ContentItems");
        }

        // 🔹 Cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // 🔹 Cifrar contraseñas (SHA256)
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
