using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

public partial class CartItem
{
    [Key]
    [Column("cartItemId")]
    public Guid CartItemId { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("productId")]
    public Guid ProductId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// Price of the product when added to the cart
    /// </summary>
    [Column("price")]
    [Precision(15, 0)]
    public decimal Price { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("CartItems")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("CartItems")]
    public virtual User User { get; set; } = null!;
}
