using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

public partial class Discount
{
    [Key]
    [Column("discountId")]
    public Guid DiscountId { get; set; }

    [Column("discountName")]
    [StringLength(100)]
    public string DiscountName { get; set; } = null!;

    [Column("description")]
    [StringLength(100)]
    public string? Description { get; set; }

    [Column("isPercentage")]
    public bool IsPercentage { get; set; }

    [Column("discountValue")]
    [Precision(15, 2)]
    public decimal DiscountValue { get; set; }

    [Column("minimumOrderAmount")]
    [Precision(15, 0)]
    public decimal? MinimumOrderAmount { get; set; }

    [Column("maximumDiscount")]
    [Precision(15, 0)]
    public decimal? MaximumDiscount { get; set; }

    [Column("fromDate", TypeName = "timestamp without time zone")]
    public DateTime? FromDate { get; set; }

    [Column("toDate", TypeName = "timestamp without time zone")]
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("createById")]
    public Guid? CreateById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    public Guid? ModifiedById { get; set; }

    [Column("usageLimit")]
    public int? UsageLimit { get; set; }

    [Column("usedCount")]
    public int? UsedCount { get; set; }

    [Column("minQuantity")]
    public int? MinQuantity { get; set; }

    [Column("maxQuantity")]
    public int? MaxQuantity { get; set; }

    [Column("isFirstOrder")]
    public bool IsFirstOrder { get; set; }

    [ForeignKey("CreateById")]
    [InverseProperty("DiscountCreateBies")]
    public virtual User? CreateBy { get; set; }

    [InverseProperty("Discount")]
    public virtual ICollection<DiscountCode> DiscountCodes { get; set; } = new List<DiscountCode>();

    [InverseProperty("Discount")]
    public virtual ICollection<DiscountProduct> DiscountProducts { get; set; } =
        new List<DiscountProduct>();

    [ForeignKey("ModifiedById")]
    [InverseProperty("DiscountModifiedBies")]
    public virtual User? ModifiedBy { get; set; }
}
