using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Setting;

public class SettingReqDto
{
    [Required(ErrorMessage = "ReviewId is required")]
    public Guid SettingId { get; set; }

    [Required(ErrorMessage = "Key is required")]
    [StringLength(255, ErrorMessage = "Value cannot exceed 255 characters")]
    public required string Value { get; set; }

    [StringLength(255, ErrorMessage = "Value cannot exceed 255 characters")]
    public string? Description { get; set; }
}
