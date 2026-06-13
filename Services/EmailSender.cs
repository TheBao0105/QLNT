using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace QLNT.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody);
}

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;

    public SmtpEmailSender(IConfiguration configuration)
    {
        _settings = new SmtpSettings();
        configuration.GetSection("SmtpSettings").Bind(_settings);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        using var message = new MailMessage();
        message.From = new MailAddress(_settings.Email, _settings.SenderName);
        message.To.Add(new MailAddress(toEmail));
        message.Subject = subject;
        message.Body = htmlBody;
        message.IsBodyHtml = true;

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSSL,
            Credentials = new NetworkCredential(_settings.Email, _settings.Password)
        };

        await client.SendMailAsync(message);
    }

    private class SmtpSettings
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 25;
        public bool EnableSSL { get; set; } = true;
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string SenderName { get; set; } = "";
    }
}
