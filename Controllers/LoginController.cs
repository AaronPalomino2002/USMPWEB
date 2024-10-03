using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using USMPWEB.Data; // Asegúrate de que apunte a tu DbContext
using USMPWEB.Models; // Donde tengas la clase Login

namespace USMPWEB.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string correo, string password)
        {
            // Busca al usuario por correo y contraseña en tu tabla 'login'
            var user = _context.DataHome.SingleOrDefault(u => u.Correo == correo && u.Password == password);

            if (user != null)
            {
                // Usuario encontrado, puedes redirigir al Home o Dashboard
                return RedirectToAction("Index", "Home");
            }

            // Si el inicio de sesión falla
            ViewBag.ErrorMessage = "Correo o contraseña incorrectos";
            return View();
        }
    }
}
