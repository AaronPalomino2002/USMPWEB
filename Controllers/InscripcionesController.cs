using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using USMPWEB.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace USMPWEB.Controllers
{
   
    public class InscripcionesController : Controller
    {
        private readonly ILogger<InscripcionesController> _logger;
        private readonly HttpClient _httpClient;

        public InscripcionesController(ILogger<InscripcionesController> logger,IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
             // Llamada a la API para obtener campañas
            var e_inscripciones = await _httpClient.GetFromJsonAsync<EventosInscripciones[]>("http://localhost:5260/api/EventosInscripciones");
            ViewData["EventosInscripciones"] = e_inscripciones; // Pasar las campañas a la vista

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}