using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using USMPWEB.Models; // Importar el modelo
using USMPWEB.Data;
using ClasificacionModelo;
using System.Threading.Tasks;
using System.Linq;

namespace USMPWEB.Controllers
{
    [Route("[controller]")]
    public class ContactoController : Controller
    {
        private readonly ILogger<ContactoController> _logger;
        private readonly ApplicationDbContext _context; // Inyectar el DbContext
        private readonly IEmailSender _emailSender;  // Agregar esto

        public ContactoController(ILogger<ContactoController> logger, ApplicationDbContext context,IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _emailSender = emailSender;
        }

        [HttpGet("Registro")]
        public IActionResult Registro()
        {
            ViewData["CurrentDateTime"] = DateTime.Now.ToString("h:mm tt - d MMMM yyyy");
            return View();
        }

        [HttpPost("Registro")]
        public async Task<IActionResult> RegistroPost(string Nombre, string Correo, int Celular, string Comentario)
        {
            // Crear una instancia del input del modelo de clasificación
            MLModelTextClassification.ModelInput sampleData = new MLModelTextClassification.ModelInput
            {
                Comentario = Comentario
            };

            // Obtener la predicción del modelo
            MLModelTextClassification.ModelOutput output = MLModelTextClassification.Predict(sampleData);

            // Obtener las etiquetas con las puntuaciones
            var sortedScoresWithLabel = MLModelTextClassification.PredictAllLabels(sampleData).ToList();
            var scoreKeyFirst = sortedScoresWithLabel[0].Key;
            var scoreValueFirst = sortedScoresWithLabel[0].Value;
            var scoreKeySecond = sortedScoresWithLabel[1].Key;
            var scoreValueSecond = sortedScoresWithLabel[1].Value;

            // Clasificar el comentario basado en la etiqueta y la puntuación
            // Clasificar el comentario basado en las puntuaciones de las etiquetas
            string clasificacionComentario = scoreValueFirst > scoreValueSecond ? (scoreKeyFirst == "1" ? "Positivo" : "Negativo") : (scoreKeySecond == "1" ? "Positivo" : "Negativo");
            double porcentaje = scoreValueFirst > scoreValueSecond ? scoreValueFirst : scoreValueSecond;


            // Mostrar en la consola si el comentario fue clasificado como positivo o negativo
            Console.WriteLine($"Comentario: {Comentario}");
            Console.WriteLine($"Clasificación: {clasificacionComentario}");
            Console.WriteLine($"Porcentaje: {porcentaje * 100}%");
            Console.WriteLine($"Primer Etiqueta: {scoreKeyFirst,-40} Puntuación: {scoreValueFirst,-20}");
            Console.WriteLine($"Segunda Etiqueta: {scoreKeySecond,-40} Puntuación: {scoreValueSecond,-20}");

            if (ModelState.IsValid)
            {
                var nuevoContacto = new Contacto
                {
                    Nombre = Nombre,
                    Correo = Correo,
                    Celular = Celular,
                    Comentario = Comentario,
                    Category = clasificacionComentario
                };

                _context.DataContacto.Add(nuevoContacto);
                await _context.SaveChangesAsync();

                // Enviar email de confirmación
                try
                {
                    await _emailSender.SendEmailAsync(
                        Correo,
                        "Confirmación de Registro de Contacto - USMP",
                        clasificacionComentario
                    );
                }
                catch (Exception ex)
                {
                    // Log el error pero no interrumpir el flujo
                    _logger.LogError(ex, "Error al enviar email de confirmación");
                }

                return RedirectToAction("Registro");
            }

            return View("Registro");
        }


        [HttpGet("error")] // Agregar este atributo
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!"); // Asegúrate de tener una vista llamada "Error"
        }
    }
}
