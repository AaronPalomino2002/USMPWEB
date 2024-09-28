using Microsoft.AspNetCore.Mvc;

namespace USMPWEB.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            // Aquí puedes agregar alguna validación básica si lo deseas
            // Por ahora, simplemente redirigimos al inicio
            return RedirectToAction("Index", "Home");
        }
    }
}