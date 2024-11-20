using System.ComponentModel.DataAnnotations;
using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.RequestDtos.Discount;

public class AddDiscountCodeReqDto
{
    public Guid? UserId { get; set; }
    
    [Required]
    [StringLength(20, MinimumLength = 4, ErrorMessage = "Discount code must be between 4 and 20 characters")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Discount code can only contain letters and numbers")]
    public required string DiscountCode { get; set; }
    
    public DateTime? FromDate { get; set; }
    
    public DateTime? ToDate { get; set; }

    public string Status { get; set; } = DiscountCodeStatus.Active;
}