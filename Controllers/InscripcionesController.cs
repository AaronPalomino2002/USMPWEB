using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using USMPWEB.Models;

namespace USMPWEB.Controllers
{
    public class InscripcionesController : Controller
    {
        private readonly ILogger<InscripcionesController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public InscripcionesController(ILogger<InscripcionesController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("API base URL not found in configuration.");
        }

        public async Task<IActionResult> Index()
        {
            // Llamada a la API usando la URL base configurada
            var e_inscripciones = await _httpClient.GetFromJsonAsync<EventosInscripciones[]>($"{_apiBaseUrl}/EventosInscripciones");
            ViewData["EventosInscripciones"] = e_inscripciones; // Pasar los datos a la vista

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
