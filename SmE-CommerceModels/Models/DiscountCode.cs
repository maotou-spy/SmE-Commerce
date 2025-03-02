using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

[Index("Status", Name = "idx_discountcodes_status")]
public partial class DiscountCode
{
    [Key]
    [Column("codeId")]
    public Guid CodeId { get; set; }

    [Column("discountId")]
    public Guid DiscountId { get; set; }

    /// <summary>
    /// this code only for this user
    /// </summary>
    [Column("userId")]
    public Guid? UserId { get; set; }

    [Column("fromDate", TypeName = "timestamp without time zone")]
    public DateTime? FromDate { get; set; }

    [Column("toDate", TypeName = "timestamp without time zone")]
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Values: active, inactive, used, deleted
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

    [Column("discountCode")]
    [StringLength(20)]
    public string Code { get; set; } = null!;

    [ForeignKey("CreateById")]
    [InverseProperty("DiscountCodeCreateBies")]
    public virtual User? CreateBy { get; set; }

    [ForeignKey("DiscountId")]
    [InverseProperty("DiscountCodes")]
    public virtual Discount Discount { get; set; } = null!;

    [ForeignKey("ModifiedById")]
    [InverseProperty("DiscountCodeModifiedBies")]
    public virtual User? ModifiedBy { get; set; }

    [InverseProperty("DiscountCode")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("UserId")]
    [InverseProperty("DiscountCodeUsers")]
    public virtual User? User { get; set; }
}
