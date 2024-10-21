using Microsoft.Extensions.Options;
using Project.Presentation.Web.Models;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.SmtpUsername),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };
        mailMessage.To.Add(email);

        using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
        {
            Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
            EnableSsl = true
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
}

