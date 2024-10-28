using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using USMPWEB.Models;

namespace USMPWEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("API base URL not found in configuration.");
        }

        public async Task<IActionResult> Index()
        {
            ViewData["CurrentDateTime"] = DateTime.Now.ToString("h:mm tt - d MMMM yyyy");

            // Llamada a la API usando la URL base configurada
            var campanas = await _httpClient.GetFromJsonAsync<Campanas[]>($"{_apiBaseUrl}/Campanas");
            ViewData["Campanas"] = campanas; // Pasar las campañas a la vista

            return View();
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
