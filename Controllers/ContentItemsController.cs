using EduMedia.Web.Data;
using EduMedia.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduMedia.Web.Controllers
{
    public class ContentItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContentItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTADO PRINCIPAL
        public async Task<IActionResult> Index(string? search, string? category, string? type)
        {
            var query = _context.ContentItems.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Titulo.ToLower().Contains(search.ToLower()));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(c => c.Categoria.ToLower() == category.ToLower());

            if (!string.IsNullOrEmpty(type))
                query = query.Where(c => c.Tipo.ToLower() == type.ToLower());

            var items = await query.ToListAsync();
            return View(items);
        }

        // FORMULARIO DE CREACIÓN
        public IActionResult Create()
        {
            return View();
        }

        // GUARDAR NUEVO CONTENIDO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContentItem item)
        {
            if (ModelState.IsValid)
            {
                // Descripción por defecto
                if (string.IsNullOrEmpty(item.Descripcion))
                    item.Descripcion = "Sin descripción";

                // Si no hay imagen, generamos una automática según el tipo de enlace
                if (string.IsNullOrEmpty(item.Imagen))
                {
                    if (!string.IsNullOrEmpty(item.Url))
                    {
                        if (item.Url.Contains("youtube.com") || item.Url.Contains("youtu.be"))
                        {
                            var videoId = GetYouTubeVideoId(item.Url);
                            if (!string.IsNullOrEmpty(videoId))
                                item.Imagen = $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";
                            else
                                item.Imagen = "https://via.placeholder.com/300x180?text=EduMedia";
                        }
                        else
                        {
                            item.Imagen = "https://via.placeholder.com/300x180?text=EduMedia";
                        }
                    }
                    else
                    {
                        item.Imagen = "https://via.placeholder.com/300x180?text=EduMedia";
                    }
                }

                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        // DETALLES
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contentItem = await _context.ContentItems.FirstOrDefaultAsync(m => m.Id == id);
            if (contentItem == null) return NotFound();

            return View(contentItem);
        }

        // ELIMINAR (Ajax o JS)
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var contentItem = await _context.ContentItems.FindAsync(id);
            if (contentItem == null) return NotFound();

            _context.ContentItems.Remove(contentItem);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Contenido eliminado correctamente." });
        }

        // MÉTODO PRIVADO: obtener el ID de un video de YouTube
        private string GetYouTubeVideoId(string url)
        {
            try
            {
                if (url.Contains("youtu.be/"))
                    return url.Split("youtu.be/")[1].Split('?')[0];
                else if (url.Contains("v="))
                    return url.Split("v=")[1].Split('&')[0];
            }
            catch { }

            return string.Empty;
        }
    }
}
