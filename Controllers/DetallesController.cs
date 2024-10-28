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

        public async Task<IActionResult> Index(int id)
        {
            // Llamada a la API para obtener los detalles de la campaña
            var campana = await _httpClient.GetFromJsonAsync<Campanas>($"{_apiBaseUrl}/Campanas/{id}");

            if (campana == null)
            {
                return NotFound();
            }

            return View(campana);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
