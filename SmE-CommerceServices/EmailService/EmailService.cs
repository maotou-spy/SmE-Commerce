using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace SmE_CommerceServices.EmailService;

public class EmailService(IConfiguration configuration) : IEmailService
{
    public async Task<bool> SendPasswordResetEmailAsync(
        string email,
        string token,
        string resetLink
    )
    {
        try
        {
            var smtpClient = new SmtpClient(configuration["Smtp:Host"])
            {
                Port = int.Parse(configuration["Smtp:Port"] ?? string.Empty),
                Credentials = new NetworkCredential(
                    configuration["Smtp:Username"],
                    configuration["Smtp:Password"]
                ),
                EnableSsl = bool.Parse(configuration["Smtp:EnableSsl"] ?? string.Empty),
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(configuration["Smtp:FromEmail"] ?? string.Empty),
                Subject = "Password Reset Request",
                Body =
                    $"<p>Click <a href='{resetLink}?token={token}'>here</a> to reset your password.</p>",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
            return false;
        }
    }
}
