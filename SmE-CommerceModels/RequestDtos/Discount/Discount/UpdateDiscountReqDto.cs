using System.ComponentModel.DataAnnotations;
using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.RequestDtos.Discount;

public class UpdateDiscountReqDto
{
    [Required(ErrorMessage = "Discount name is required")]
    public required string DiscountName { get; set; }

    public string? Description { get; set; }

    public decimal? MinimumOrderAmount { get; set; }

    public decimal? MaximumDiscount { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int? UsageLimit { get; set; }

    public int? MinQuantity { get; set; }

    public int? MaxQuantity { get; set; }

    public bool IsFirstOrder { get; set; } = false;

    public List<Guid> ProductIds { get; set; } = [];
}