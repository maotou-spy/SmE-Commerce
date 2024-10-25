using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.User
{
    public class CreateUserReqDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Must be email format")]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;
    }
}
