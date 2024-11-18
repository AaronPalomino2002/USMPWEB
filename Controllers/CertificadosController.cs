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
    public class CertificadosController : Controller
    {
        private readonly ILogger<CertificadosController> _logger;
        private readonly ApplicationDbContext _context;

        public CertificadosController(ILogger<CertificadosController> logger,  ApplicationDbContext context)
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
                var certificados = from c in _context.DataCertificados
                               select c;

                // Filtro de búsqueda
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var searchTermLower = searchTerm.ToLower(); // Convertir el término de búsqueda a minúsculas
                    certificados = certificados.Where(c =>
                        c.NombreCertificado.ToLower().Contains(searchTermLower) ||
                        c.Descripcion.ToLower().Contains(searchTermLower)
                    );
                    ViewData["CurrentSearch"] = searchTerm;
                }

                // Filtro de fecha
                var fechaHoy = DateOnly.FromDateTime(DateTime.Today);
                switch (dateFilter)
                {
                    case "urgente":
                        certificados = certificados.Where(c => c.FechaInicio <= fechaHoy.AddDays(1));
                        break;
                    case "desdeAyer":
                        certificados = certificados.Where(c => c.FechaInicio >= fechaHoy.AddDays(-1));
                        break;
                    case "ultimos3Dias":
                        certificados = certificados.Where(c => c.FechaInicio >= fechaHoy.AddDays(-3));
                        break;
                    case "ultimaSemana":
                        certificados = certificados.Where(c => c.FechaInicio >= fechaHoy.AddDays(-7));
                        break;
                }
                ViewData["CurrentDate"] = dateFilter;

                // Filtro de categoría
                if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter != "todos")
                {
                    certificados = certificados.Where(c => c.CategoriaId.ToString() == categoryFilter);
                    ViewData["CurrentCategory"] = categoryFilter;
                }

                // Filtro de carrera (actualizado para usar SubCategorias)
                // if (!string.IsNullOrEmpty(careerFilter) && careerFilter != "todos")
                // {
                //     long careerFilterId = long.Parse(careerFilter);
                //     certificados = certificados.Where(c => c.SubCategorias.Any(sc => sc.IdSubCategoria == careerFilterId));
                //     ViewData["CurrentCareer"] = careerFilter;
                // }

                // Ordenamiento
                switch (sortOrder)
                {
                    case "fecha_desc":
                        certificados = certificados.OrderByDescending(c => c.FechaInicio);
                        break;
                    case "fecha_asc":
                        certificados = certificados.OrderBy(c => c.FechaInicio);
                        break;
                    case "titulo_desc":
                        certificados = certificados.OrderByDescending(c => c.NombreCertificado);
                        break;
                    case "titulo_asc":
                        certificados = certificados.OrderBy(c => c.NombreCertificado);
                        break;
                    default:
                        certificados = certificados.OrderByDescending(c => c.FechaInicio);
                        break;
                }
                ViewData["CurrentSort"] = sortOrder;

                // Cargar datos relacionados y ejecutar la consulta
                var resultados = await certificados
                    .Include(c => c.Categoria)
                    .Include(c => c.SubCategorias)  // Cambiado de SubCategoria a SubCategorias
                    .ToListAsync();

                ViewData["Certificados"] = resultados.ToArray();

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
