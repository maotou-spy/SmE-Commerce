using System;
using System.Collections.Generic;
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

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("price")]
    [Precision(15, 0)]
    public decimal Price { get; set; }

    [Column("variantId")]
    public Guid VariantId { get; set; }

    [Column("productName")]
    [StringLength(100)]
    public string ProductName { get; set; } = null!;

    [Column("variantName")]
    [StringLength(100)]
    public string? VariantName { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderItems")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("VariantId")]
    [InverseProperty("OrderItems")]
    public virtual ProductVariant Variant { get; set; } = null!;
}
