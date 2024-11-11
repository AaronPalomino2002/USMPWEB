using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using USMPWEB.Models; // Importar el modelo
using USMPWEB.Data;
using ClasificacionModelo;


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

            //FALTA CONVERTIR LOS PARAMETROS EN OBJETOS PARA HACER EL LLMADO AL MENSAJE

            MLModelTextClassification.ModelInput sampleData = new MLModelTextClassification.ModelInput(){
                // Comentario = 
            };
             MLModelTextClassification.ModelOutput output = MLModelTextClassification.Predict(sampleData);
            //   Console.WriteLine($"{output.Label}{output.PredictedLabel}");

             // output.Score.ToList().ForEach(score => Console.WriteLine(score));

            var sortedScoresWithLabel = MLModelTextClassification.PredictAllLabels(sampleData);
            var scoreKey = sortedScoresWithLabel.ToList()[0].Key;
            var scoreValueFirst = sortedScoresWithLabel.ToList()[0].Value;
            var scoreKeySecond = sortedScoresWithLabel.ToList()[1].Key;
            var scoreValueSecond = sortedScoresWithLabel.ToList()[1].Value;
            // foreach(var score in sortedScoresWithLabel){
            //     Console.WriteLine($"{score.Key,-40}{score.Value,-20}");
            // }

            if(scoreKey == "1"){
                    
            if(scoreValueFirst > 0.5){
                Comentario = "Negativo";
            }
            else{
                Comentario = "Positivo";
            }

            }
            
                Console.WriteLine($"{scoreKey,-40}{scoreValueFirst,-20}");
                Console.WriteLine($"{scoreKeySecond,-40}{scoreValueSecond,-20}");


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

        [HttpGet("error")] // Agregar este atributo
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!"); // Asegúrate de tener una vista llamada "Error"
        }
    }
}
