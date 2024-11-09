using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using USMPWEB.Models;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
    {
        _smtpSettings = smtpSettings.Value;
        _logger = logger;
    }

    public async Task SendReceiptEmailAsync(string toEmail, string customerName, Pago pago)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = $"Recibo de Pago - {pago.NumeroRecibo}";

            var builder = new BodyBuilder();
            builder.HtmlBody = GetEmailTemplate(customerName, pago);

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending receipt email");
            throw;
        }
    }

    private string GetEmailTemplate(string customerName, Pago pago)
    {
        var template = pago.MetodoPago == "PagoEfectivo" 
            ? GetPagoEfectivoTemplate(customerName, pago)
            : GetPagoTarjetaTemplate(customerName, pago);
        
        return template;
    }

    private string GetPagoEfectivoTemplate(string customerName, Pago pago)
    {
        return $@"
        <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #003366;'>Recibo para Pago en Efectivo - USMP</h2>
                    <p>Estimado(a) {customerName},</p>
                    <p>Se ha generado su recibo para pago en efectivo:</p>
                    
                    <div style='background-color: #f8f9fa; padding: 15px; margin: 15px 0;'>
                        <h3>Detalles del Pago</h3>
                        <p><strong>Número de Recibo:</strong> {pago.NumeroRecibo}</p>
                        <p><strong>Monto a Pagar:</strong> S/. {pago.Monto:N2}</p>
                        <p><strong>Fecha límite:</strong> {pago.FechaExpiracion?.ToString("dd/MM/yyyy HH:mm")}</p>
                    </div>

                    <div style='background-color: #fff3cd; padding: 15px; margin: 15px 0;'>
                        <h4>Instrucciones de Pago:</h4>
                        <ol>
                            <li>Acérquese a cualquier agencia bancaria o agente</li>
                            <li>Realice el pago por el monto indicado</li>
                            <li>Conserve su comprobante de pago</li>
                        </ol>
                    </div>

                    <p><strong>Nota:</strong> El pago debe realizarse antes de la fecha límite indicada.</p>
                </div>
            </body>
        </html>";
    }

    private string GetPagoTarjetaTemplate(string customerName, Pago pago)
    {
        return $@"
        <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #f8f9fa; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; }}
                    .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #6c757d; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>Recibo de Pago</h2>
                    </div>
                    <div class='content'>
                        <p>Estimado(a) {customerName},</p>
                        <p>Gracias por tu pago. Aquí están los detalles de tu transacción:</p>
                        
                        <h3>Detalles del Pago</h3>
                        <p>Número de Recibo: {pago.NumeroRecibo}</p>
                        <p>Fecha: {pago.FechaPago?.ToString("dd/MM/yyyy HH:mm:ss")}</p>
                        <p>Monto: S/. {pago.Monto:N2}</p>
                        <p>Estado: {pago.Estado}</p>

                        <p>Este correo es una confirmación automática, por favor no responder.</p>
                    </div>
                    <div class='footer'>
                        <p>© {DateTime.Now.Year} USMP. Todos los derechos reservados.</p>
                    </div>
                </div>
            </body>
        </html>";
    }
}