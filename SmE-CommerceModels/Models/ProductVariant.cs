using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

public partial class ProductVariant
{
    [Key]
    [Column("productVariantId")]
    public Guid ProductVariantId { get; set; }

    [Column("productId")]
    public Guid ProductId { get; set; }

    [Column("variantId")]
    public Guid VariantId { get; set; }

    [Column("attributeValue")]
    [StringLength(100)]
    public string AttributeValue { get; set; } = null!;

    [Column("sku")]
    [StringLength(50)]
    public string? Sku { get; set; }

    [Column("price")]
    [Precision(15, 0)]
    public decimal Price { get; set; }

    [Column("stockQuantity")]
    public int StockQuantity { get; set; }

    [Column("soldQuantity")]
    public int SoldQuantity { get; set; }

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

    [ForeignKey("ProductVariantId")]
    [InverseProperty("ProductVariants")]
    public virtual VariantAttribute Attribute { get; set; } = null!;

    [ForeignKey("CreateById")]
    [InverseProperty("ProductVariantCreateBies")]
    public virtual User? CreateBy { get; set; }

    [ForeignKey("ModifiedById")]
    [InverseProperty("ProductVariantModifiedBies")]
    public virtual User? ModifiedBy { get; set; }

    [InverseProperty("Variant")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [ForeignKey("ProductId")]
    [InverseProperty("ProductVariants")]
    public virtual Product Product { get; set; } = null!;
}
