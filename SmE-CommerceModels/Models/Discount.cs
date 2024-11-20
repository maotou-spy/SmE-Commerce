namespace SmE_CommerceModels.Models;

public class Discount
{
    public Guid DiscountId { get; set; }

    public string DiscountName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsPercentage { get; set; }

    public decimal DiscountValue { get; set; }

    public decimal? MinimumOrderAmount { get; set; }

    public decimal? MaximumDiscount { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public Guid? CreateById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public Guid? ModifiedById { get; set; }

    public int? UsageLimit { get; set; }

    public int? UsedCount { get; set; }

    public int? MinQuantity { get; set; }

    public int? MaxQuantity { get; set; }

    public bool? IsFirstOrder { get; set; }

    public virtual User? CreateBy { get; set; }

    public virtual ICollection<DiscountCode> DiscountCodes { get; set; } = new List<DiscountCode>();

    public virtual ICollection<DiscountProduct> DiscountProducts { get; set; } = new List<DiscountProduct>();

    public virtual User? ModifiedBy { get; set; }
}
