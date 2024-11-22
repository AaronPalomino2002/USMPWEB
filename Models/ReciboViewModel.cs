using System;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace USMPWEB.Models
{
    public class ReciboViewModel
    {
        public int InscripcionId { get; set; }
        public string? NumeroRecibo { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Matricula { get; set; }
        public string? Facultad { get; set; }
        public string? Carrera { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public Campanas? Campana { get; set; }
        public Certificados? Certificado { get; set; }
        public EventosInscripciones? Evento { get; set; }
        // Agregar propiedad para identificar el tipo
        public string? TipoInscripcion { get; set; }
        public string? Estado { get; set; }
        public string? QRCodeImage { get; set; }
        public void GenerarQR()
        {
            try
            {
                if (string.IsNullOrEmpty(NumeroRecibo))
                {
                    // Si no hay número de recibo, usar otro identificador único
                    NumeroRecibo = $"REC-{DateTime.Now:yyyyMMddHHmmss}-{InscripcionId}";
                }

                using var qrGenerator = new QRCodeGenerator();
                // Crear un texto que incluya más información para el QR
                var qrText = $"Recibo:{NumeroRecibo}|ID:{InscripcionId}|Fecha:{FechaInscripcion:yyyy-MM-dd}";
                var qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeImage = qrCode.GetGraphic(20);
                QRCodeImage = $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
            }
            catch (Exception ex)
            {
                // En caso de error, establecer una imagen por defecto o manejar el error
                QRCodeImage = null;
                // Podrías loggear el error si tienes acceso a un logger
            }
        }
    }
}