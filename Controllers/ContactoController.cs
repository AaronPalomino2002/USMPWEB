using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace USMPWEB.Controllers
{
    [Route("[controller]")]
    public class ContactoController : Controller
    {
        private readonly ILogger<ContactoController> _logger;

        public ContactoController(ILogger<ContactoController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Registro")]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost("Registro")]
        public IActionResult RegistroPost(string Nombre, string Correo, int Celular ,string Comentario)
        {
            if (ModelState.IsValid)
            {
                // Procesar los datos recibidos
                return RedirectToAction("Muchas Gracias"); // Redireccionar a otra página o acción
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}