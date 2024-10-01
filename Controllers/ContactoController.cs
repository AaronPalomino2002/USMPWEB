using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using USMPWEB.Models; // Importar el modelo
using USMPWEB.Data; // Importar el contexto de la base de datos

namespace USMPWEB.Controllers
{
    [Route("[controller]")]
    public class ContactoController : Controller
    {
        private readonly ILogger<ContactoController> _logger;
        private readonly ApplicationDbContext _context; // Inyectar el DbContext

        public ContactoController(ILogger<ContactoController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("Registro")]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost("Registro")]
        public async Task<IActionResult> RegistroPost(string Nombre, string Correo, int Celular, string Comentario)
        {
            if (ModelState.IsValid)
            {
                // Crear una nueva instancia del modelo Contacto
                var nuevoContacto = new Contacto
                {
                    Nombre = Nombre,
                    Correo = Correo,
                    Celular = Celular,
                    Comentario = Comentario
                };

                // Agregar a la base de datos
                _context.DataContacto.Add(nuevoContacto);
                await _context.SaveChangesAsync(); // Guardar los cambios en la base de datos

                // Redirigir a la misma página del formulario para limpiar los campos
                return RedirectToAction("Registro");
            }

            // Si el modelo no es válido, simplemente devolver la vista con los errores
            return View("Registro");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
