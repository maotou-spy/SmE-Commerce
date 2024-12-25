using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

public partial class OrderItem
{
    [Key]
    [Column("orderItemId")]
    public Guid OrderItemId { get; set; }

    [Column("orderId")]
    public Guid OrderId { get; set; }

    [Column("productId")]
    public Guid ProductId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("price")]
    [Precision(15, 0)]
    public decimal Price { get; set; }

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("variantId")]
    public Guid VariantId { get; set; }

    [Column("productName")]
    [StringLength(100)]
    public string ProductName { get; set; } = null!;

    [Column("attributeValue")]
    [StringLength(100)]
    public string? AttributeValue { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderItems")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("OrderItems")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("VariantNameId")]
    [InverseProperty("OrderItems")]
    public virtual ProductVariant Variant { get; set; } = null!;
}
