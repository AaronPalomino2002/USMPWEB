using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore; // Para trabajar con Entity Framework
using System.Linq;
using System.Threading.Tasks;
using USMPWEB.Data; // Asegúrate de que apunte a tu DbContext
using USMPWEB.Models; // Donde tengas la clase Login y el modelo de registro

namespace USMPWEB.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string ADMIN_EMAIL = "juan@usmp.pe";

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
            // Busca al alumno por correo y contraseña en la tabla 'alumnos'
            var alumno = await _context.DataAlumnos
                .Include(a => a.Login) // Asegúrate de incluir la relación con Login
                .SingleOrDefaultAsync(a => a.Correo == correo && a.Login.Password == password); // Usa la relación

            if (alumno != null)
            {
                // Usuario encontrado, puedes autenticar al usuario
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, alumno.Nombre + " " + alumno.ApePat + " " + alumno.ApeMat), // Puedes modificar esto según tus necesidades
                    // Agregar un claim para identificar si es admin
                    new Claim("IsAdmin", (correo == ADMIN_EMAIL).ToString())

                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // Puedes cambiar esto según tus necesidades
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Tiempo de expiración
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                // Redirigir al Home o Dashboard
                if (correo == ADMIN_EMAIL)
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "Home");
            }

            // Si el inicio de sesión falla
            ViewBag.ErrorMessage = "Correo o contraseña incorrectos";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home"); // Redirigir a la página principal después de cerrar sesión
        }

        // Acción para mostrar el formulario de registro
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Carreras = _context.DataCarrera.ToList(); // Cargar las carreras para el select
            return View();
        }

        // Acción para procesar el registro de un nuevo usuario
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe un usuario con el mismo correo
                var existingUser = await _context.DataAlumnos.SingleOrDefaultAsync(u => u.Correo == model.Correo);
                if (existingUser != null)
                {
                    // Ya existe un usuario con ese correo
                    ViewBag.ErrorMessage = "Este correo ya está registrado.";
                    ViewBag.Carreras = _context.DataCarrera.ToList(); // Cargar las carreras
                    return View(model);
                }

                try
                {
                    var newRegistro = new RegisterViewModel
                    {
                        numMatricula = model.numMatricula,
                        Nombre = model.Nombre,
                        apePat = model.apePat,
                        apeMat = model.apeMat,
                        Correo = model.Correo,
                        Edad = model.Edad,
                        Celular = model.Celular,
                        Password = model.Password,
                        ConfirmPassword = model.ConfirmPassword,
                        CarreraId = model.CarreraId,
                    };
                     // Guardar en la tabla de 'Registro'
                    _context.DataRegistro.Add(newRegistro);
                    await _context.SaveChangesAsync();

                    // Crear una nueva instancia de la entidad Login con los datos de registro
                    var newUser = new Login
                    {
                        Correo = model.Correo,
                        Password = model.Password // Considera encriptar la contraseña antes de guardarla
                    };

                    // Guardar en la tabla de 'login'
                    _context.DataHome.Add(newUser);
                    await _context.SaveChangesAsync(); // Guarda primero el login para obtener el ID

                    // Crear una nueva instancia de la entidad Alumno
                    var newAlumno = new Alumno
                    {
                        NumMatricula = model.numMatricula,
                        Nombre = model.Nombre,
                        ApePat = model.apePat,
                        ApeMat = model.apeMat,
                        Correo = model.Correo,
                        Edad = model.Edad,
                        Celular = model.Celular,
                        CarreraId = model.CarreraId,
                        // Establecer la relación con el Login
                        Login = newUser // Asocia el nuevo Login
                    };

                    // Guardar en la tabla de 'alumnos'
                    _context.DataAlumnos.Add(newAlumno);
                    await _context.SaveChangesAsync();

                    // Redirigir al login o a una página de confirmación
                    return RedirectToAction("Index", "Login");
                }
                catch (Exception ex)
                {
                    // Log o manejar el error
                    ViewBag.ErrorMessage = "Ocurrió un error al registrar el usuario: " + ex.Message;
                    ViewBag.Carreras = _context.DataCarrera.ToList(); // Cargar las carreras
                    return View(model);
                }
            }

            // Si el modelo no es válido, recargar las carreras
            ViewBag.Carreras = _context.DataCarrera.ToList();
            return View(model);
        }
    }
}
