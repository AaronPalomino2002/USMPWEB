using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using USMPWEB.Data;
using USMPWEB.Models;

namespace USMPWEB.Controllers
{
    public class InscripcionesController : Controller
    {
        private readonly ILogger<InscripcionesController> _logger;
        private readonly ApplicationDbContext _context;

        public InscripcionesController(ILogger<InscripcionesController> logger, IConfiguration configuration, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

       [HttpGet]
        public async Task<IActionResult> Index(string searchTerm, string sortOrder, string dateFilter,
                                         string categoryFilter, string careerFilter)
        {
            ViewData["CurrentDateTime"] = DateTime.Now.ToString("h:mm tt - d MMMM yyyy");

            try
            {
                var e_inscripciones = from c in _context.DataEventosInscripciones
                               select c;

                // Filtro de búsqueda
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var searchTermLower = searchTerm.ToLower(); // Convertir el término de búsqueda a minúsculas
                    e_inscripciones = e_inscripciones.Where(e =>
                        e.Titulo.ToLower().Contains(searchTermLower) ||
                        e.Descripcion.ToLower().Contains(searchTermLower)
                    );
                    ViewData["CurrentSearch"] = searchTerm;
                }

                // Filtro de fecha
                var fechaHoy = DateOnly.FromDateTime(DateTime.Today);
                switch (dateFilter)
                {
                    case "urgente":
                        e_inscripciones = e_inscripciones.Where(c => c.FechaInicio <= fechaHoy.AddDays(1));
                        break;
                    case "desdeAyer":
                        e_inscripciones = e_inscripciones.Where(c => c.FechaInicio >= fechaHoy.AddDays(-1));
                        break;
                    case "ultimos3Dias":
                        e_inscripciones = e_inscripciones.Where(c => c.FechaInicio >= fechaHoy.AddDays(-3));
                        break;
                    case "ultimaSemana":
                        e_inscripciones = e_inscripciones.Where(c => c.FechaInicio >= fechaHoy.AddDays(-7));
                        break;
                }
                ViewData["CurrentDate"] = dateFilter;

                // Filtro de categoría
                if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter != "todos")
                {
                    e_inscripciones = e_inscripciones.Where(c => c.CategoriaId.ToString() == categoryFilter);
                    ViewData["CurrentCategory"] = categoryFilter;
                }

                // Filtro de carrera (actualizado para usar SubCategorias)
                // if (!string.IsNullOrEmpty(careerFilter) && careerFilter != "todos")
                // {
                //     long careerFilterId = long.Parse(careerFilter);
                //     e_inscripciones = e_inscripciones.Where(c => c.SubCategorias.Any(sc => sc.IdSubCategoria == careerFilterId));
                //     ViewData["CurrentCareer"] = careerFilter;
                // }

                // Ordenamiento
                switch (sortOrder)
                {
                    case "fecha_desc":
                        e_inscripciones = e_inscripciones.OrderByDescending(c => c.FechaInicio);
                        break;
                    case "fecha_asc":
                        e_inscripciones = e_inscripciones.OrderBy(c => c.FechaInicio);
                        break;
                    case "titulo_desc":
                        e_inscripciones = e_inscripciones.OrderByDescending(c => c.Titulo);
                        break;
                    case "titulo_asc":
                        e_inscripciones = e_inscripciones.OrderBy(c => c.Titulo);
                        break;
                    default:
                        e_inscripciones = e_inscripciones.OrderByDescending(c => c.FechaInicio);
                        break;
                }
                ViewData["CurrentSort"] = sortOrder;

                // Cargar datos relacionados y ejecutar la consulta
                var resultados = await e_inscripciones
                    .Include(e => e.Categoria)
                    .Include(e => e.SubCategorias)  // Cambiado de SubCategoria a SubCategorias
                    .ToListAsync();

                ViewData["EventosInscripciones"] = resultados.ToArray();

                // Cargar datos para los dropdowns
                ViewData["Categorias"] = await _context.DataCategoria.ToListAsync();
                ViewData["Carreras"] = await _context.DataCarrera.ToListAsync();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar las campañas");
                TempData["Error"] = "Error al cargar las campañas. Por favor, intente nuevamente.";
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
