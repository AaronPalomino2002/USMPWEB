using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using USMPWEB.Data;
using USMPWEB.Models;

namespace USMPWEB.Controllers
{
    public class DetallesController : Controller

    {
        private readonly ILogger<DetallesController> _logger;
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context; // Agregar esto

        public DetallesController(
            ILogger<DetallesController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ApplicationDbContext context) // Agregar esto
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _context = context; // Agregar esto
            var baseUrl = configuration["ApiSettings:BaseUrl"] ??
                throw new InvalidOperationException("API base URL not found in configuration.");
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<IActionResult> Index(int id, string tipo)
        {
            try
            {
                if (string.IsNullOrEmpty(tipo))
                {
                    return BadRequest("El tipo no puede estar vacío.");
                }

                if (tipo.Equals("campana", StringComparison.OrdinalIgnoreCase))
                {
                    // Intentar obtener la campaña directamente de la base de datos
                    var campana = await _context.DataCampanas
                        .Include(c => c.Categoria)
                        .Include(c => c.SubCategorias)
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (campana == null)
                    {
                        _logger.LogWarning($"Campaña con ID {id} no encontrada");
                        return NotFound($"No se encontró la campaña con ID {id}");
                    }

                    return View("~/Views/Home/Detalles/Index.cshtml", campana);
                }
                else if (tipo.Equals("inscripcion", StringComparison.OrdinalIgnoreCase))
                {
                    var e_inscripcion = await _context.DataEventosInscripciones
                        .Include(e => e.Categoria)
                        .FirstOrDefaultAsync(e => e.Id == id);

                    if (e_inscripcion == null)
                    {
                        _logger.LogWarning($"Inscripción con ID {id} no encontrada");
                        return NotFound($"No se encontró la inscripción con ID {id}");
                    }

                    return View("~/Views/Inscripciones/Detalles/Index.cshtml", e_inscripcion);
                }
                else if (tipo.Equals("certificados", StringComparison.OrdinalIgnoreCase))
                {
                    var certificados = await _context.DataCertificados
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (certificados == null)
                    {
                        _logger.LogWarning($"Certificado con ID {id} no encontrado");
                        return NotFound($"No se encontró el certificado con ID {id}");
                    }

                    return View("~/Views/Certificados/Detalles/Index.cshtml", certificados);
                }
                else
                {
                    return BadRequest("Tipo no válido. Debe ser 'campana', 'inscripcion' o 'certificados'.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener detalles para tipo {tipo} con ID {id}");
                // Devolver una vista de error personalizada con más información
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Ha ocurrido un error al cargar los detalles. Por favor, intente nuevamente.",
                    DetailedMessage = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> InscribirCertificado(
            string Nombres,
            string Apellidos,
            string Matricula,
            string Facultad,
            string Carrera,
            string Direccion,
            string Telefono,
            string Email,
            int CertificadoId,
            bool AceptoTerminos)
        {
            try
            {
                var certificado = await _context.DataCertificados
                    .Include(c => c.Categoria)
                    .Include(c => c.SubCategoria)
                    .FirstOrDefaultAsync(c => c.Id == CertificadoId);

                if (certificado == null)
                {
                    TempData["Error"] = "Certificado no encontrado";
                    return RedirectToAction("Index", new { id = CertificadoId, tipo = "certificados" });
                }

                // Crear la inscripción
                var inscripcion = new CertificadoInscripcion
                {
                    CertificadoId = CertificadoId,
                    NumeroRecibo = $"CERT-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4)}",
                    Nombres = Nombres,
                    Apellidos = Apellidos,
                    Matricula = Matricula,
                    Facultad = Facultad,
                    Carrera = Carrera,
                    Direccion = Direccion,
                    Telefono = Telefono,
                    Email = Email,
                    Monto = 90.00M,
                    FechaInscripcion = DateTime.UtcNow,
                    Estado = "Pendiente",
                    AceptoTerminos = AceptoTerminos
                };

                _context.CertificadoInscripciones.Add(inscripcion);
                await _context.SaveChangesAsync();

                // Crear el recibo
                var recibo = new ReciboViewModel
                {
                    InscripcionId = inscripcion.Id,
                    NumeroRecibo = inscripcion.NumeroRecibo,
                    Nombres = inscripcion.Nombres,
                    Apellidos = inscripcion.Apellidos,
                    Matricula = inscripcion.Matricula,
                    Facultad = inscripcion.Facultad,
                    Carrera = inscripcion.Carrera,
                    Email = inscripcion.Email,
                    Direccion = inscripcion.Direccion,
                    Telefono = inscripcion.Telefono,
                    Monto = inscripcion.Monto,
                    FechaInscripcion = inscripcion.FechaInscripcion.ToLocalTime(),
                    Certificado = certificado,
                    Estado = inscripcion.Estado,
                    TipoInscripcion = "Certificado" // Indicar que es una inscripción de certificado
                };

                recibo.GenerarQR();

                TempData["Recibo"] = JsonSerializer.Serialize(recibo);
                return RedirectToAction("MostrarReciboCertificado", new { id = inscripcion.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la inscripción");
                TempData["Error"] = "Ocurrió un error al procesar tu inscripción";
                return RedirectToAction("Index", new { id = CertificadoId, tipo = "certificados" });
            }
        }

        [HttpGet]
        public IActionResult MostrarReciboCertificado(int id)
        {
            try
            {
                if (TempData["Recibo"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var recibo = JsonSerializer.Deserialize<ReciboViewModel>((string)TempData["Recibo"]);
                return View("~/Views/Home/Detalles/MostrarRecibo.cshtml", recibo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mostrar el recibo");
                TempData["Error"] = "Ocurrió un error al mostrar el recibo";
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> InscribirEvento(
    string Nombres,
    string Apellidos,
    string Matricula,
    string Facultad,
    string Carrera,
    string Direccion,
    string Telefono,
    string Email,
    int EventoId,
    bool AceptoTerminos)
        {
            try
            {
                var evento = await _context.DataEventosInscripciones
                    .Include(e => e.Categoria)
                    .Include(e => e.SubCategorias)
                    .FirstOrDefaultAsync(e => e.Id == EventoId);

                if (evento == null)
                {
                    TempData["Error"] = "Evento no encontrado";
                    return RedirectToAction("Index", new { id = EventoId, tipo = "inscripcion" });
                }

                var inscripcion = new EventoInscripcion
                {
                    EventoId = EventoId,
                    NumeroRecibo = $"EVT-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4)}",
                    Nombres = Nombres,
                    Apellidos = Apellidos,
                    Matricula = Matricula,
                    Facultad = Facultad,
                    Carrera = Carrera,
                    Direccion = Direccion,
                    Telefono = Telefono,
                    Email = Email,
                    Monto = 90.00M,
                    FechaInscripcion = DateTime.UtcNow,
                    Estado = "Pendiente",
                    AceptoTerminos = AceptoTerminos
                };

                _context.EventoInscripciones.Add(inscripcion);
                await _context.SaveChangesAsync();

                var recibo = new ReciboViewModel
                {
                    InscripcionId = inscripcion.Id,
                    NumeroRecibo = inscripcion.NumeroRecibo,
                    Nombres = inscripcion.Nombres,
                    Apellidos = inscripcion.Apellidos,
                    Matricula = inscripcion.Matricula,
                    Facultad = inscripcion.Facultad,
                    Carrera = inscripcion.Carrera,
                    Email = inscripcion.Email,
                    Direccion = inscripcion.Direccion,
                    Telefono = inscripcion.Telefono,
                    Monto = inscripcion.Monto,
                    FechaInscripcion = inscripcion.FechaInscripcion.ToLocalTime(),
                    Evento = evento,
                    Estado = inscripcion.Estado,
                    TipoInscripcion = "Evento"
                };

                recibo.GenerarQR();

                TempData["Recibo"] = JsonSerializer.Serialize(recibo);
                return RedirectToAction("MostrarRecibo", new { id = inscripcion.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la inscripción al evento");
                TempData["Error"] = "Ocurrió un error al procesar tu inscripción";
                return RedirectToAction("Index", new { id = EventoId, tipo = "inscripcion" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Inscribirse(
            string Nombres,
            string Apellidos,
            string Matricula,
            string Facultad,
            string Carrera,
            string Direccion,
            string Telefono,
            string Email,
            int CampanaId,
            bool AceptoTerminos)
        {
            try
            {
                // Obtener la campaña con todos sus datos relacionados
                var campana = await _context.DataCampanas
                    .Include(c => c.Categoria)
                    .Include(c => c.SubCategorias)
                    .FirstOrDefaultAsync(c => c.Id == CampanaId);

                if (campana == null)
                {
                    TempData["Error"] = "La campaña no existe";
                    return RedirectToAction("Index", new { id = CampanaId, tipo = "campana" });
                }

                // Crear la inscripción
                var inscripcion = new CampanaInscripcion
                {
                    CampanaId = CampanaId,
                    NumeroRecibo = $"REC-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4)}", // Generar número de recibo
                    Nombres = Nombres,
                    Apellidos = Apellidos,
                    Matricula = Matricula,
                    Facultad = Facultad,
                    Carrera = Carrera,
                    Direccion = Direccion,
                    Telefono = Telefono,
                    Email = Email,
                    Monto = 90.00M,
                    FechaInscripcion = DateTime.UtcNow,
                    Estado = "Pendiente",
                    AceptoTerminos = AceptoTerminos
                };

                _context.CampanaInscripciones.Add(inscripcion);
                await _context.SaveChangesAsync();

                // Crear el recibo
                var recibo = new ReciboViewModel
                {
                    InscripcionId = inscripcion.Id,
                    NumeroRecibo = $"REC-{DateTime.Now:yyyyMMdd}-{inscripcion.Id:D4}",
                    Nombres = inscripcion.Nombres,
                    Apellidos = inscripcion.Apellidos,
                    Matricula = inscripcion.Matricula,
                    Facultad = inscripcion.Facultad,
                    Carrera = inscripcion.Carrera,
                    Email = inscripcion.Email,
                    Direccion = inscripcion.Direccion,
                    Telefono = inscripcion.Telefono,
                    Monto = inscripcion.Monto,
                    FechaInscripcion = inscripcion.FechaInscripcion.ToLocalTime(),
                    Campana = campana,
                    Estado = inscripcion.Estado
                };

                // Generar código QR
                recibo.GenerarQR(); // Cambiado de GenerarCodigos() a GenerarQR()

                TempData["Recibo"] = JsonSerializer.Serialize(recibo, new JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
                });

                return RedirectToAction(nameof(MostrarRecibo), new { id = inscripcion.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la inscripción");
                TempData["Error"] = "Ocurrió un error al procesar tu inscripción";
                return RedirectToAction("Index", new { id = CampanaId, tipo = "campana" });
            }
        }

        [HttpGet]
        public IActionResult MostrarRecibo(int id)
        {
            try
            {
                if (TempData["Recibo"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var recibo = JsonSerializer.Deserialize<ReciboViewModel>((string)TempData["Recibo"]);
                return View("~/Views/Home/Detalles/MostrarRecibo.cshtml", recibo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mostrar el recibo");
                TempData["Error"] = "Ocurrió un error al mostrar el recibo";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult CerrarRecibo(string returnUrl = null)
        {
            return Redirect(returnUrl ?? "/");
        }
    }

}