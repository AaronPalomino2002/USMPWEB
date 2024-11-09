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
        // Agregar propiedad para identificar el tipo
        public string? TipoInscripcion { get; set; } // "Campana" o "Certificado"
        public string? Estado { get; set; }
        public string? QRCodeImage { get; set; }
        public void GenerarQR()
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(NumeroRecibo, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);
            QRCodeImage = $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
        }
    }
}