using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Address;

public class AddressReqDto
{
    [Required]
    public string ReceiverName { get; set; } = null!;

    [Required]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must start with 0 and be exactly 10 digits.")]
    public string ReceiverPhone { get; set; } = null!;

    [Required]
    public string Address { get; set; } = null!;

    [Required]
    public string Ward { get; set; } = null!;

    [Required]
    public string District { get; set; } = null!;

    [Required]
    public string City { get; set; } = null!;

    [Required]
    public bool IsDefault { get; set; } = false;
}
