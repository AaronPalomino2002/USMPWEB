using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using USMPWEB.Models;

namespace USMPWEB.Controllers
{
    public class DetallesController : Controller
    {
        private readonly ILogger<DetallesController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public DetallesController(ILogger<DetallesController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("API base URL not found in configuration.");
        }

        public async Task<IActionResult> Index(int id, string tipo)
        {
            if (string.IsNullOrEmpty(tipo))
            {
                return BadRequest("El tipo no puede estar vacío.");
            }

            if (tipo.Equals("campana", StringComparison.OrdinalIgnoreCase))
            {
                // Llamada a la API para obtener los detalles de la campaña
                var campana = await _httpClient.GetFromJsonAsync<Campanas>($"{_apiBaseUrl}/Campanas/{id}");

                if (campana == null)
                {
                    return NotFound();
                }

                return View("~/Views/Home/Detalles/Index.cshtml", campana); // Ruta completa a la vista de Campanas
            }
            else if (tipo.Equals("inscripcion", StringComparison.OrdinalIgnoreCase))
            {
                // Llamada a la API para obtener los detalles de la inscripción
                var e_inscripcion = await _httpClient.GetFromJsonAsync<EventosInscripciones>($"{_apiBaseUrl}/EventosInscripciones/{id}");

                if (e_inscripcion == null)
                {
                    return NotFound();
                }

                return View("~/Views/Inscripciones/Detalles/Index.cshtml", e_inscripcion); // Ruta completa a la vista de Inscripciones
            }
            else if (tipo.Equals("certificados", StringComparison.OrdinalIgnoreCase))
            {
                // Llamada a la API para obtener los detalles de la inscripción
                var certificados = await _httpClient.GetFromJsonAsync<Certificados>($"{_apiBaseUrl}/Certificados/{id}");

                if (certificados == null)
                {
                    return NotFound();
                }

                return View("~/Views/Certificados/Detalles/Index.cshtml", certificados); // Ruta completa a la vista de Inscripciones
            }
            else
            {
                return BadRequest("Tipo no válido. Debe ser 'campana', 'inscripcion', 'certficados' .");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
