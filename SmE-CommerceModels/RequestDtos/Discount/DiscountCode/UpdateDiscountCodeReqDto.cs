using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Discount.DiscountCode;

public class UpdateDiscountCodeReqDto
{
    [Required]
    [StringLength(20, MinimumLength = 4, ErrorMessage = "Discount code must be between 4 and 20 characters")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Discount code can only contain letters and numbers")]
    public required string DiscountCode { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }
}