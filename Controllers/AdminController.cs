using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using USMPWEB.Models;
using USMPWEB.Data;

namespace USMPWEB.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;
        private const string ADMIN_EMAIL = "juan@usmp.pe";

        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated ||
                User.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value != "True")
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Alumnos()
        {
            if (!User.Identity.IsAuthenticated ||
                User.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value != "True")
            {
                return RedirectToAction("Index", "Login");
            }

            // Corregido para solo incluir Login, ya que CarreraId es una FK
            var alumnos = await _context.DataAlumnos
                .Include(a => a.Login)
                .Include(a => a.Carrera)  // Incluir la relación con Carrera
                .ToListAsync();

            return View(alumnos);
        }

        [HttpGet]
        public async Task<IActionResult> EditarAlumno(int id)
        {
            var alumno = await _context.DataAlumnos
                .Include(a => a.Login)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (alumno == null)
            {
                return NotFound();
            }

            // Cargar las carreras para el dropdown
            ViewBag.Carreras = await _context.DataCarrera.ToListAsync();
            return View(alumno);
        }

        [HttpPost]
        public async Task<IActionResult> EditarAlumno(int id, Alumno alumno)
        {
            if (id != alumno.Id)
            {
                return NotFound();
            }

            try
            {
                var alumnoExistente = await _context.DataAlumnos
                    .Include(a => a.Login)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (alumnoExistente == null)
                {
                    return NotFound();
                }

                // Actualizar datos del alumno
                alumnoExistente.NumMatricula = alumno.NumMatricula;
                alumnoExistente.Nombre = alumno.Nombre;
                alumnoExistente.ApePat = alumno.ApePat;
                alumnoExistente.ApeMat = alumno.ApeMat;
                alumnoExistente.Correo = alumno.Correo;
                alumnoExistente.Edad = alumno.Edad;
                alumnoExistente.Celular = alumno.Celular;
                alumnoExistente.CarreraId = alumno.CarreraId;

                if (!string.IsNullOrEmpty(alumno.Login?.Password))
                {
                    alumnoExistente.Login.Password = alumno.Login.Password;
                }

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Alumno actualizado correctamente";
                return RedirectToAction(nameof(Alumnos));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar el alumno: " + ex.Message;
                ViewBag.Carreras = await _context.DataCarrera.ToListAsync();
                return View(alumno);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarAlumno(int id)
        {
            try
            {
                var alumno = await _context.DataAlumnos
                    .Include(a => a.Login)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (alumno == null)
                {
                    TempData["Error"] = "Alumno no encontrado";
                    return RedirectToAction(nameof(Alumnos));
                }

                // Verificar si tiene inscripciones
                var tieneInscripciones = await _context.DataInscripciones
                    .AnyAsync(i => i.Alumno == alumno.NumMatricula);

                if (tieneInscripciones)
                {
                    TempData["Error"] = "No se puede eliminar el alumno porque tiene inscripciones registradas";
                    return RedirectToAction(nameof(Alumnos));
                }

                if (alumno.Login != null)
                {
                    _context.DataHome.Remove(alumno.Login);
                }

                _context.DataAlumnos.Remove(alumno);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Alumno eliminado correctamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el alumno: " + ex.Message;
                _logger.LogError(ex, "Error al eliminar alumno ID: {Id}", id);
            }

            return RedirectToAction(nameof(Alumnos));
        }
        [HttpGet]
        public async Task<IActionResult> Campanas()
        {
            if (!User.Identity.IsAuthenticated ||
                User.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value != "True")
            {
                return RedirectToAction("Index", "Login");
            }

            var campanas = await _context.DataCampanas
                .Include(c => c.Categoria)
                .Include(c => c.SubCategoria)
                .OrderByDescending(c => c.FechaInicio)
                .ToListAsync();

            return View(campanas);
        }
        [HttpGet]
        public async Task<IActionResult> EditarCampana(int id)
        {
            var campana = await _context.DataCampanas
                .Include(c => c.Categoria)
                .Include(c => c.SubCategoria)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (campana == null)
            {
                return NotFound();
            }

            // Cargar categorías y subcategorías para los dropdowns
            ViewBag.Categorias = await _context.DataCategoria.ToListAsync();
            ViewBag.SubCategorias = await _context.DataSubCategoria.ToListAsync();
            return View(campana);
        }

        [HttpPost]
        public async Task<IActionResult> EditarCampana(int id, Campanas campana)
        {
            if (id != campana.Id)
            {
                return NotFound();
            }

            try
            {
                var campanaExistente = await _context.DataCampanas
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (campanaExistente == null)
                {
                    return NotFound();
                }

                // Actualizar datos de la campaña
                campanaExistente.Titulo = campana.Titulo;
                campanaExistente.Descripcion = campana.Descripcion;
                campanaExistente.CategoriaId = campana.CategoriaId;
                campanaExistente.subCategoriaId = campana.subCategoriaId;
                campanaExistente.Imagen = campana.Imagen;
                campanaExistente.FechaInicio = campana.FechaInicio;
                campanaExistente.FechaFin = campana.FechaFin;

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Campaña actualizada correctamente";
                return RedirectToAction(nameof(Campanas));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar la campaña: " + ex.Message;
                ViewBag.Categorias = await _context.DataCategoria.ToListAsync();
                ViewBag.SubCategorias = await _context.DataSubCategoria.ToListAsync();
                return View(campana);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarCampana(int id)
        {
            try
            {
                var campana = await _context.DataCampanas
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (campana == null)
                {
                    TempData["Error"] = "Campaña no encontrada";
                    return RedirectToAction(nameof(Campanas));
                }

                // Verificar si hay inscripciones relacionadas
                var tieneInscripciones = await _context.DataInscripciones
                    .AnyAsync(i => i.Alumno == campana.Titulo);

                if (tieneInscripciones)
                {
                    TempData["Error"] = "No se puede eliminar la campaña porque tiene inscripciones registradas";
                    return RedirectToAction(nameof(Campanas));
                }

                _context.DataCampanas.Remove(campana);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Campaña eliminada correctamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la campaña: " + ex.Message;
                _logger.LogError(ex, "Error al eliminar campaña ID: {Id}", id);
            }

            return RedirectToAction(nameof(Campanas));
        }
    }
}