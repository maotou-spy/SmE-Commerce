using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Auth;

public partial class LoginWithAccountReqDto
{
    [Required(ErrorMessage = "Email or phone is required")]
    public required string EmailOrPhone { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public required string Password { get; set; }
}
