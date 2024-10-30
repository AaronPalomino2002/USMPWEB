using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using USMPWEB.Models;
using USMPWEB.Data;

namespace USMPWEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
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
                var campanas = from c in _context.DataCampanas
                              select c;

                // Filtro de búsqueda
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    campanas = campanas.Where(c => c.Titulo.Contains(searchTerm) || 
                                                 c.Descripcion.Contains(searchTerm));
                    ViewData["CurrentSearch"] = searchTerm;
                }

                // Filtro de fecha
                var fechaHoy = DateOnly.FromDateTime(DateTime.Today);
                switch (dateFilter)
                {
                    case "urgente":
                        campanas = campanas.Where(c => c.FechaInicio <= fechaHoy.AddDays(1));
                        break;
                    case "desdeAyer":
                        campanas = campanas.Where(c => c.FechaInicio >= fechaHoy.AddDays(-1));
                        break;
                    case "ultimos3Dias":
                        campanas = campanas.Where(c => c.FechaInicio >= fechaHoy.AddDays(-3));
                        break;
                    case "ultimaSemana":
                        campanas = campanas.Where(c => c.FechaInicio >= fechaHoy.AddDays(-7));
                        break;
                }
                ViewData["CurrentDate"] = dateFilter;

                // Filtro de categoría
                if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter != "todos")
                {
                    campanas = campanas.Where(c => c.CategoriaId.ToString() == categoryFilter);
                    ViewData["CurrentCategory"] = categoryFilter;
                }

                // Filtro de carrera
                if (!string.IsNullOrEmpty(careerFilter) && careerFilter != "todos")
                {
                    campanas = campanas.Where(c => c.subCategoriaId.ToString() == careerFilter);
                    ViewData["CurrentCareer"] = careerFilter;
                }

                // Ordenamiento
                switch (sortOrder)
                {
                    case "fecha_desc":
                        campanas = campanas.OrderByDescending(c => c.FechaInicio);
                        break;
                    case "fecha_asc":
                        campanas = campanas.OrderBy(c => c.FechaInicio);
                        break;
                    case "titulo_desc":
                        campanas = campanas.OrderByDescending(c => c.Titulo);
                        break;
                    case "titulo_asc":
                        campanas = campanas.OrderBy(c => c.Titulo);
                        break;
                    default:
                        campanas = campanas.OrderByDescending(c => c.FechaInicio);
                        break;
                }
                ViewData["CurrentSort"] = sortOrder;

                // Cargar datos relacionados y ejecutar la consulta
                var resultados = await campanas
                    .Include(c => c.Categoria)
                    .Include(c => c.SubCategoria)
                    .ToListAsync();

                ViewData["Campanas"] = resultados.ToArray();

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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}