using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using USMPWEB.Data; // Asegúrate de que apunte a tu DbContext
using USMPWEB.Models; // Donde tengas la clase Login y el modelo de registro

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

        // Acción para mostrar el formulario de registro
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Acción para procesar el registro de un nuevo usuario
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe un usuario con el mismo correo
                var existingUser = _context.DataHome.SingleOrDefault(u => u.Correo == model.Correo);
                if (existingUser != null)
                {
                    // Ya existe un usuario con ese correo
                    ViewBag.ErrorMessage = "Este correo ya está registrado.";
                    return View(model);
                }

                // Crear una nueva instancia de la entidad Login con los datos de registro
                var newUser = new Login
                {
                    Correo = model.Correo,
                    Password = model.Password
                };

                // Guardar en la tabla de 'login'
                _context.DataHome.Add(newUser);
                await _context.SaveChangesAsync();

                // Redirigir al login o a una página de confirmación
                return RedirectToAction("Index", "Login");
            }

            return View(model);
        }
    }
}
