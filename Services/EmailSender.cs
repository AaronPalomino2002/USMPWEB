using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using USMPWEB.Models;

namespace USMPWEB.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<SmtpSettings> smtpSettings, ILogger<EmailSender> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try 
            {
                using var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                    Subject = "Confirmación de Registro de Contacto - USMP",
                    IsBodyHtml = true,
                    Body = GetContactConfirmationTemplate(message)
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email de confirmación enviado a {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email de confirmación de contacto");
                throw;
            }
        }

        private string GetContactConfirmationTemplate(string clasificacionComentario)
        {
            return $@"
            <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background-color: #ff0033; padding: 20px; text-align: center;'>
                            <h2 style='color: white; margin: 0;'>Confirmación de Registro</h2>
                        </div>
                        
                        <div style='background-color: #f8f9fa; padding: 20px; margin-top: 20px;'>
                            <p>¡Gracias por contactarnos!</p>
                            <p>Hemos recibido tu mensaje correctamente. Nuestro equipo revisará tu comentario y te responderemos a la brevedad posible.</p>
                            
                            <div style='background-color: {(clasificacionComentario.Contains("Positivo") ? "#e8f5e9" : "#ffebee")}; 
                                      padding: 15px; 
                                      margin: 15px 0; 
                                      border-radius: 5px;'>
                                <p style='margin: 0;'>
                                    <strong>Clasificación de tu comentario:</strong> {clasificacionComentario}
                                </p>
                            </div>

                            <p>Detalles importantes:</p>
                            <ul>
                                <li>Tu mensaje ha sido registrado en nuestro sistema</li>
                                <li>Te responderemos al correo electrónico proporcionado</li>
                                <li>Tiempo estimado de respuesta: 24-48 horas hábiles</li>
                            </ul>
                        </div>
                        
                        <div style='text-align: center; margin-top: 20px; color: #666;'>
                            <p>© USMP ${DateTime.Now.Year} - Todos los derechos reservados</p>
                        </div>
                    </div>
                </body>
            </html>";
        }
    }
}