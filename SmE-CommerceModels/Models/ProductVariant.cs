using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

public class ProductVariant
{
    [Key]
    [Column("productVariantId")]
    public Guid ProductVariantId { get; set; }

    [Column("productId")]
    public Guid ProductId { get; set; }

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
    public DateTime CreatedAt { get; set; }

    [Column("createById")]
    [StringLength(50)]
    public required string CreateById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    [StringLength(50)]
    public string? ModifiedById { get; set; }

    [Column("variantImage")]
    [StringLength(255)]
    public string? VariantImage { get; set; }

    [InverseProperty("ProductVariant")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [InverseProperty("ProductVariant")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [ForeignKey("ProductId")]
    [InverseProperty("ProductVariants")]
    public virtual Product Product { get; set; } = null!;

    [InverseProperty("ProductVariant")]
    public virtual ICollection<VariantAttribute> VariantAttributes { get; set; } =
        new List<VariantAttribute>();
}
