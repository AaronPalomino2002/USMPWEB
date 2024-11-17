using USMPWEB.Models;

public interface IEmailSender
{
    Task SendEmailAsync(string toEmail, string subject, string message);
}

