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
}
}