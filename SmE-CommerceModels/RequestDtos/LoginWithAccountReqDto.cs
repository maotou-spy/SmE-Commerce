using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos;

public class LoginWithAccountReqDto
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+~\-={}[\]:;""'<>?,.\/]).{8,}$",
        ErrorMessage =
            "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and must be at least 8 characters long.")]
    public string Password { get; set; }
}
