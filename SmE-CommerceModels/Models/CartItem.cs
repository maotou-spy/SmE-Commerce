﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

public class CartItem
{
    [Key]
    [Column("cartItemId")]
    public Guid CartItemId { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("productVariantId")]
    public Guid? ProductVariantId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    /// <summary>
    ///     Price of the product when added to the cart
    /// </summary>
    [Column("price")]
    [Precision(15, 0)]
    public decimal Price { get; set; }

    [Column("productId")]
    public Guid ProductId { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("CartItems")]
    public virtual Product Product { get; set; } = null!;
    
    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("ProductVariantId")]
    [InverseProperty("CartItems")]
    public virtual ProductVariant? ProductVariant { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("CartItems")]
    public virtual User User { get; set; } = null!;
}
