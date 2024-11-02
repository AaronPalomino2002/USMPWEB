using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using USMPWEB.Data;
using USMPWEB.Models;

namespace USMPWEB.Controllers
{
    public class DetallesController : Controller
    {
        private readonly ILogger<DetallesController> _logger;
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context; // Agregar esto

        public DetallesController(
            ILogger<DetallesController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ApplicationDbContext context) // Agregar esto
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _context = context; // Agregar esto
            var baseUrl = configuration["ApiSettings:BaseUrl"] ??
                throw new InvalidOperationException("API base URL not found in configuration.");
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<IActionResult> Index(int id, string tipo)
        {
            try
            {
                if (string.IsNullOrEmpty(tipo))
                {
                    return BadRequest("El tipo no puede estar vacío.");
                }

                if (tipo.Equals("campana", StringComparison.OrdinalIgnoreCase))
                {
                    // Intentar obtener la campaña directamente de la base de datos
                    var campana = await _context.DataCampanas
                        .Include(c => c.Categoria)
                        .Include(c => c.SubCategorias)
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (campana == null)
                    {
                        _logger.LogWarning($"Campaña con ID {id} no encontrada");
                        return NotFound($"No se encontró la campaña con ID {id}");
                    }

                    return View("~/Views/Home/Detalles/Index.cshtml", campana);
                }
                else if (tipo.Equals("inscripcion", StringComparison.OrdinalIgnoreCase))
                {
                    var e_inscripcion = await _context.DataEventosInscripciones
                        .Include(e => e.Categoria)
                        .FirstOrDefaultAsync(e => e.Id == id);

                    if (e_inscripcion == null)
                    {
                        _logger.LogWarning($"Inscripción con ID {id} no encontrada");
                        return NotFound($"No se encontró la inscripción con ID {id}");
                    }

                    return View("~/Views/Inscripciones/Detalles/Index.cshtml", e_inscripcion);
                }
                else if (tipo.Equals("certificados", StringComparison.OrdinalIgnoreCase))
                {
                    var certificados = await _context.DataCertificados
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (certificados == null)
                    {
                        _logger.LogWarning($"Certificado con ID {id} no encontrado");
                        return NotFound($"No se encontró el certificado con ID {id}");
                    }

                    return View("~/Views/Certificados/Detalles/Index.cshtml", certificados);
                }
                else
                {
                    return BadRequest("Tipo no válido. Debe ser 'campana', 'inscripcion' o 'certificados'.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener detalles para tipo {tipo} con ID {id}");
                // Devolver una vista de error personalizada con más información
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Ha ocurrido un error al cargar los detalles. Por favor, intente nuevamente.",
                    DetailedMessage = ex.Message
                });
            }
        }
    }
}