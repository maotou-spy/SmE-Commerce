using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Auth;

public class RegisterWithAccountReqDto
{
    [Required]
    [RegularExpression(
        @"^[a-zA-Z\s]+$",
        ErrorMessage = "Full name can only contain letters and spaces."
    )]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [Phone]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits.")]
    [RegularExpression(
        @"^0\d{9}$",
        ErrorMessage = "Phone number must start with 0 and be exactly 10 digits."
    )]
    public string Phone { get; set; } = null!;

    [Required]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+~\-={}[\]:;""'<>?,.\/]).{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and must be at least 8 characters long."
    )]
    public string Password { get; set; } = null!;
    
    [Required]
    [Compare("Password", ErrorMessage = "Confirm password does not match.")]
    public string ConfirmPassword { get; set; } = null!;
}
