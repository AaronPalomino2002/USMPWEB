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
                .Include(c => c.SubCategorias)
                .OrderByDescending(c => c.FechaInicio)
                .ToListAsync();

            return View(campanas);
        }

        [HttpGet]
        public async Task<IActionResult> EventosInscripciones()
        {
            if (!User.Identity.IsAuthenticated ||
                User.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value != "True")
            {
                return RedirectToAction("Index", "Login");
            }

            var e_inscripciones = await _context.DataEventosInscripciones
                .Include(c => c.Categoria)
                .Include(c => c.SubCategoria)
                .OrderByDescending(c => c.FechaInicio)
                .ToListAsync();

            return View(e_inscripciones);
        }

        [HttpGet]
        public IActionResult CrearEventoInscripciones()
        {
            // Cargar categorías y subcategorías desde la base de datos
            var categorias = _context.DataCategoria.ToList();
            var subCategorias = _context.DataSubCategoria.ToList();

            ViewBag.Categoria = categorias ?? new List<Categoria>();          // Cargar categorías en ViewBag
            ViewBag.SubCategoria = subCategorias ?? new List<SubCategoria>(); // Cargar subcategorías en ViewBag

            return View();
        }


        // Acción para mostrar el formulario de creación de campaña
        [HttpGet]
        public async Task<IActionResult> CrearCampana()
        {
            ViewBag.Categoria = await _context.DataCategoria.ToListAsync();
            ViewBag.SubCategoria = await _context.DataSubCategoria.ToListAsync();
            return View(new Campanas());
        }

        [HttpPost]
        public async Task<IActionResult> CrearCampana(Campanas campana)
        {
            try
            {
                if (campana.FechaFin < campana.FechaInicio)
                {
                    ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser anterior a la fecha de inicio");
                }

                if (campana.SubCategoriaIds == null || campana.SubCategoriaIds.Count < 1 || campana.SubCategoriaIds.Count > 3)
                {
                    ModelState.AddModelError("SubCategoriaIds", "Debe seleccionar entre 1 y 3 subcategorías");
                }

                if (!ModelState.IsValid)
                {
                    // Recargar las listas para el formulario
                    ViewBag.Categoria = await _context.DataCategoria.ToListAsync();
                    ViewBag.SubCategoria = await _context.DataSubCategoria.ToListAsync();
                    return View(campana);
                }

                // Obtener las subcategorías
                var subcategorias = await _context.DataSubCategoria
                    .Where(s => campana.SubCategoriaIds.Contains(s.IdSubCategoria))
                    .ToListAsync();

                campana.SubCategorias = subcategorias;

                // Asegurarnos que el Id sea 0
                campana.Id = 0;

                await _context.DataCampanas.AddAsync(campana);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Campaña creada correctamente";
                return RedirectToAction(nameof(Campanas));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la campaña");
                TempData["Error"] = "Error al crear la campaña: " + ex.Message;
                ViewBag.Categoria = await _context.DataCategoria.ToListAsync();
                ViewBag.SubCategoria = await _context.DataSubCategoria.ToListAsync();
                return View(campana);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditarCampana(int id)
        {
            var campana = await _context.DataCampanas
                .Include(c => c.Categoria)
                .Include(c => c.SubCategorias)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (campana == null)
            {
                return NotFound();
            }

            ViewBag.Categorias = await _context.DataCategoria.ToListAsync();
            ViewBag.SubCategorias = await _context.DataSubCategoria.ToListAsync();

            return View(campana);
        }
        [HttpPost]
        public async Task<IActionResult> EditarCampana(int id, Campanas campana, List<long> SubCategoriaIds)
        {
            if (id != campana.Id)
            {
                return NotFound();
            }

            try
            {
                var campanaExistente = await _context.DataCampanas
                    .Include(c => c.SubCategorias)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (campanaExistente == null)
                {
                    return NotFound();
                }

                // Actualizar propiedades básicas
                campanaExistente.Titulo = campana.Titulo;
                campanaExistente.Descripcion = campana.Descripcion;
                campanaExistente.CategoriaId = campana.CategoriaId;
                campanaExistente.Imagen = campana.Imagen;
                campanaExistente.FechaInicio = campana.FechaInicio;
                campanaExistente.FechaFin = campana.FechaFin;
                // Agregar los nuevos campos
                campanaExistente.Requisitos = campana.Requisitos;
                campanaExistente.Monto = campana.Monto;

                // Actualizar subcategorías
                campanaExistente.SubCategorias.Clear();
                var subcategoriasSeleccionadas = await _context.DataSubCategoria
                    .Where(s => SubCategoriaIds.Contains(s.IdSubCategoria))
                    .ToListAsync();
                foreach (var subcat in subcategoriasSeleccionadas)
                {
                    campanaExistente.SubCategorias.Add(subcat);
                }

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
        [HttpGet]
        public async Task<IActionResult> Inscripciones()
        {
            if (!User.Identity.IsAuthenticated ||
                User.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value != "True")
            {
                return RedirectToAction("Index", "Login");
            }

            var inscripciones = await _context.DataInscripciones
                .OrderByDescending(i => i.Id)
                .ToListAsync();

            return View(inscripciones);
        }

        [HttpGet]
        public IActionResult CrearInscripcion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearInscripcion(Inscripciones inscripcion)
        {
            try
            {
                _context.DataInscripciones.Add(inscripcion);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Inscripción creada correctamente";
                return RedirectToAction(nameof(Inscripciones));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear la inscripción: " + ex.Message;
                return View(inscripcion);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditarInscripcion(long id)
        {
            var inscripcion = await _context.DataInscripciones
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inscripcion == null)
            {
                return NotFound();
            }

            return View(inscripcion);
        }

        [HttpPost]
        public async Task<IActionResult> EditarInscripcion(long id, Inscripciones inscripcion)
        {
            if (id != inscripcion.Id)
            {
                return NotFound();
            }

            try
            {
                var inscripcionExistente = await _context.DataInscripciones
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (inscripcionExistente == null)
                {
                    return NotFound();
                }

                inscripcionExistente.Alumno = inscripcion.Alumno;
                inscripcionExistente.Proceso = inscripcion.Proceso;
                inscripcionExistente.Culminado = inscripcion.Culminado;

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Inscripción actualizada correctamente";
                return RedirectToAction(nameof(Inscripciones));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar la inscripción: " + ex.Message;
                return View(inscripcion);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarInscripcion(long id)
        {
            try
            {
                var inscripcion = await _context.DataInscripciones
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (inscripcion == null)
                {
                    TempData["Error"] = "Inscripción no encontrada";
                    return RedirectToAction(nameof(Inscripciones));
                }

                _context.DataInscripciones.Remove(inscripcion);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Inscripción eliminada correctamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la inscripción: " + ex.Message;
                _logger.LogError(ex, "Error al eliminar inscripción ID: {Id}", id);
            }

            return RedirectToAction(nameof(Inscripciones));
        }
        [HttpGet]
        public async Task<IActionResult> Certificados()
        {
            if (!User.Identity.IsAuthenticated ||
                User.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value != "True")
            {
                return RedirectToAction("Index", "Login");
            }

            var certificados = await _context.DataCertificados
                .Include(c => c.Categoria)
                .Include(c => c.SubCategoria)
                .OrderByDescending(c => c.FechaExpedicion)
                .ToListAsync();


            return View(certificados);
        }

        [HttpGet]
        public IActionResult CrearCertificado()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearCertificado(Certificados certificado)
        {
            try
            {
                _context.DataCertificados.Add(certificado);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Certificado creado correctamente";
                return RedirectToAction(nameof(Certificados));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear el certificado: " + ex.Message;
                return View(certificado);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditarCertificado(long id)
        {
            var certificado = await _context.DataCertificados
                .FirstOrDefaultAsync(c => c.Id == id);

            if (certificado == null)
            {
                return NotFound();
            }

            return View(certificado);
        }

        [HttpPost]
        public async Task<IActionResult> EditarCertificado(long id, Certificados certificado)
        {
            if (id != certificado.Id)
            {
                return NotFound();
            }

            try
            {
                var certificadoExistente = await _context.DataCertificados
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (certificadoExistente == null)
                {
                    return NotFound();
                }

                certificadoExistente.NombreCertificado = certificado.NombreCertificado;
                certificadoExistente.FechaExpedicion = certificado.FechaExpedicion;

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Certificado actualizado correctamente";
                return RedirectToAction(nameof(Certificados));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar el certificado: " + ex.Message;
                return View(certificado);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarCertificado(long id)
        {
            try
            {
                var certificado = await _context.DataCertificados
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (certificado == null)
                {
                    TempData["Error"] = "Certificado no encontrado";
                    return RedirectToAction(nameof(Certificados));
                }

                _context.DataCertificados.Remove(certificado);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Certificado eliminado correctamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el certificado: " + ex.Message;
                _logger.LogError(ex, "Error al eliminar certificado ID: {Id}", id);
            }

            return RedirectToAction(nameof(Certificados));
        }
    }
}