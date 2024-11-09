using USMPWEB.Models;

public interface IEmailService
{
    Task SendReceiptEmailAsync(string toEmail, string customerName, Pago pago);
}

