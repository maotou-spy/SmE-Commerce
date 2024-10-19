using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Auth;

public class LoginWithAccountReqDto
{
    [Required]
    [Display(Name = "Email or Phone Number")]
    public string EmailOrPhone { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+~\-={}[\]:;""'<>?,.\/]).{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and must be at least 8 characters long.")]
    public string Password { get; set; }

    [CustomValidation(typeof(LoginWithAccountReqDto), nameof(ValidateEmailOrPhone))]
    public static ValidationResult ValidateEmailOrPhone(string emailOrPhone, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(emailOrPhone))
        {
            return new ValidationResult("Email or phone number cannot be empty");
        }

        bool isEmail = new EmailAddressAttribute().IsValid(emailOrPhone);
        bool isPhone = new PhoneAttribute().IsValid(emailOrPhone);

        if (!isEmail && !isPhone)
        {
            return new ValidationResult("Please enter a valid email or Vietnamese phone number");
        }

        if (isPhone && !IsValidVietnamesePhoneNumber(emailOrPhone))
        {
            return new ValidationResult("The phone number is not in a valid Vietnamese format");
        }

        return ValidationResult.Success;
    }

    private static bool IsValidVietnamesePhoneNumber(string phoneNumber)
    {
        // Regex cho số điện thoại Việt Nam
        return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^(0|\+84)(\s|\.)?((3[2-9])|(5[689])|(7[06-9])|(8[1-689])|(9[0-46-9]))(\d)(\s|\.)?(\d{3})(\s|\.)?(\d{3})$");
    }
}