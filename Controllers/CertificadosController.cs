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
   
    public class CertificadosController : Controller
    {
        private readonly ILogger<CertificadosController> _logger;
        private readonly HttpClient _httpClient;

        public CertificadosController(ILogger<CertificadosController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
             var certificados = await _httpClient.GetFromJsonAsync<Certificados[]>("http://localhost:5260/api/Certificados");
            ViewData["Certificados"] = certificados; // Pasar las campañas a la vista

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}