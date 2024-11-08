using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.Text.Json;
using USMPWEB.Data;
using Microsoft.EntityFrameworkCore;
using USMPWEB.Models;

namespace USMPWEB.Controllers
{
    public class PagosController : Controller
    {
        private readonly ILogger<PagosController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // Credenciales de prueba de Izipay
        private const string TEST_SHOP_ID = "89289758";
        private const string TEST_KEY = "testprivatekey_DEMOPRIVATEKEY95me92597fd28tGD4r5FrKfn4GSWS";
        private const string TEST_ENDPOINT = "https://api.micuentaweb.pe";

        public PagosController(
            ILogger<PagosController> logger,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> IniciarPagoTarjeta(int inscripcionId)
        {
            try
            {
                var inscripcion = await _context.CampanaInscripciones
                    .Include(i => i.Campana)
                    .FirstOrDefaultAsync(i => i.Id == inscripcionId);

                if (inscripcion == null)
                {
                    return NotFound("Inscripción no encontrada");
                }

                // Crear el formulario de pago
                var orderId = $"INS-{inscripcionId}-{DateTime.Now:yyyyMMddHHmmss}";
                var amount = (int)(inscripcion.Monto * 100); // Izipay requiere el monto en centavos

                var formData = new Dictionary<string, string>
        {
            { "shopId", TEST_SHOP_ID },
            { "orderRef", orderId },
            { "amount", amount.ToString() },
            { "currency", "PEN" },
            { "customer.email", inscripcion.Email },
            { "customer.reference", inscripcion.Matricula },
            { "customer.firstName", inscripcion.Nombres },
            { "customer.lastName", inscripcion.Apellidos },
            { "customer.phone", inscripcion.Telefono },
            { "orderDetails", $"Pago de inscripción - {inscripcion.Campana?.Titulo}" }
        };

                // Guardar información del pago
                var pago = new Pago
                {
                    InscripcionId = inscripcionId,
                    NumeroRecibo = orderId,
                    Monto = inscripcion.Monto,
                    Estado = "Pendiente",
                    MetodoPago = "Tarjeta",
                    FechaCreacion = DateTime.UtcNow,
                    FechaExpiracion = DateTime.UtcNow.AddHours(1)
                };

                _context.Pagos.Add(pago);
                await _context.SaveChangesAsync();

                // Retornar la vista con el modelo actualizado
                return View("ProcesarPagoTarjeta", new PagoTarjetaViewModel
                {
                    Monto = inscripcion.Monto,
                    NombreCliente = $"{inscripcion.Nombres} {inscripcion.Apellidos}",
                    Email = inscripcion.Email,
                    NumeroRecibo = orderId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar el pago con tarjeta");
                return BadRequest($"Error al procesar el pago: {ex.Message}");
            }
        }

        private string GenerateFormToken(Dictionary<string, string> formData)
        {
            // Ordenar los campos alfabéticamente
            var sortedData = formData.OrderBy(x => x.Key);

            // Concatenar los valores
            var dataToHash = string.Join("+", sortedData.Select(x => x.Value));

            // Agregar la clave privada
            dataToHash += "+" + TEST_KEY;

            // Generar el hash SHA256
            using (var sha256Hash = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [HttpPost]
        public async Task<IActionResult> IzipayCallback([FromBody] IzipayResponse response)
        {
            try
            {
                var pago = await _context.Pagos
                    .Include(p => p.Inscripcion)
                    .FirstOrDefaultAsync(p => p.NumeroRecibo == response.OrderId);

                if (pago != null)
                {
                    pago.Estado = response.Status == "PAID" ? "Pagado" : "Fallido";
                    pago.FechaPago = response.TransactionDate;

                    if (pago.Inscripcion != null)
                    {
                        pago.Inscripcion.Estado = response.Status == "PAID" ? "Pagado" : "Pendiente";
                    }

                    await _context.SaveChangesAsync();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar callback de Izipay");
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<IActionResult> ProcesarPago(PagoTarjetaViewModel modelo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("ProcesarPagoTarjeta", modelo);
                }

                // Buscar el pago por el número de recibo
                var pago = await _context.Pagos
                    .Include(p => p.Inscripcion)
                    .FirstOrDefaultAsync(p => p.NumeroRecibo == modelo.NumeroRecibo);

                if (pago != null)
                {
                    pago.Estado = "Pagado";
                    pago.FechaPago = DateTime.UtcNow;

                    if (pago.Inscripcion != null)
                    {
                        pago.Inscripcion.Estado = "Pagado";
                    }

                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Pago procesado correctamente";
                    return RedirectToAction("ConfirmacionPago", new { id = pago.Id });
                }

                TempData["Error"] = "No se encontró el pago";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el pago");
                TempData["Error"] = "Error al procesar el pago";
                return View("ProcesarPagoTarjeta", modelo);
            }
        }

        public IActionResult ConfirmacionPago(int id)
        {
            var pago = _context.Pagos
                .Include(p => p.Inscripcion)
                .FirstOrDefault(p => p.Id == id);

            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

    }
}

