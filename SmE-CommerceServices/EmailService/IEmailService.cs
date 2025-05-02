namespace SmE_CommerceServices.EmailService;

public interface IEmailService
{
    Task<bool> SendPasswordResetEmailAsync(string email, string token, string resetLink);
}
