using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Auth;

public partial class LoginWithAccountReqDto
{
    [Required(ErrorMessage = "Email or phone is required")]
    public required string EmailOrPhone { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+~\-={}[\]:;""'<>?,.\/]).{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and must be at least 8 characters long.")]
    public required string Password { get; set; }
}
