using System.ComponentModel.DataAnnotations;
using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.RequestDtos.Discount;

public class AddDiscountReqDto
{
    [Required(ErrorMessage = "Discount name is required")]
    public required string DiscountName { get; set; }
    
    public string? Description { get; set; }

    public bool IsPercentage { get; set; } = false; // giảm giá tiền theo % hay theo số tiền
    
    [Required]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Please enter a number")]
    public required decimal DiscountValue { get; set; }
    
    public decimal? MinimumOrderAmount { get; set; }

    public decimal? MaximumDiscount { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }
    
    public string Status { get; set; } = GeneralStatus.Active;
    
    public int? UsageLimit { get; set; }

    public int UsedCount { get; set; } = 0;
    
    public int? MinQuantity { get; set; }

    public int? MaxQuantity { get; set; }
    
    public bool IsFirstOrder { get; set; } = false;

    public List<Guid> ProductIds { get; set; } = [];
}