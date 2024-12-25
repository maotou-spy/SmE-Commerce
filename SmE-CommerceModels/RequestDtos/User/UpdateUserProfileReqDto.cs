using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.User;

public class UpdateUserProfileReqDto
{
    [Required]
    [RegularExpression(
        @"^[a-zA-Z\s]+$",
        ErrorMessage = "Full name can only contain letters and spaces."
    )]
    public required string FullName { get; set; }

    [Required]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number invalid")]
    public required string Phone { get; set; }

    [Required]
    public required string Gender { get; set; }

    [Required]
    public required DateOnly Dob { get; set; }
}
