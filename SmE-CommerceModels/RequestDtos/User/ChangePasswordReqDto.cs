using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.User;

public class ChangePasswordReqDto
{
    [Required(ErrorMessage = "Old password is required.")]
    public required string OldPassword { get; set; }

    [Required]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+~\-={}[\]:;""'<>?,.\/]).{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and must be at least 8 characters long."
    )]
    public required string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public required string ConfirmPassword { get; set; }
}
