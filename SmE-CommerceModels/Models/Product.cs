using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

[Index("Status", Name = "idx_products_status")]
[Index("ProductCode", Name = "products_code_key", IsUnique = true)]
public class Product
{
    [Key]
    [Column("productId")]
    public Guid ProductId { get; set; }

    [Column("productCode")]
    [StringLength(50)]
    public string ProductCode { get; set; } = null!;

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("price")]
    [Precision(15, 0)]
    public decimal Price { get; set; }

    [Column("stockQuantity")]
    public int StockQuantity { get; set; }

    [Column("soldQuantity")]
    public int SoldQuantity { get; set; }

    /// <summary>
    ///     Values: active, inactive, deleted
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

    [Column("slug")]
    [StringLength(255)]
    public string Slug { get; set; } = null!;

    [Column("metaTitle")]
    [StringLength(255)]
    public string MetaTitle { get; set; } = null!;

    [Column("metaDescription")]
    [StringLength(300)]
    public string? MetaDescription { get; set; }

    [Column("isTopSeller")]
    public bool IsTopSeller { get; set; }

    [Column("primaryImage")]
    [StringLength(255)]
    public string PrimaryImage { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<ContentProduct> ContentProducts { get; set; } =
        new List<ContentProduct>();

    [ForeignKey("CreateById")]
    [InverseProperty("ProductCreateBies")]
    public virtual User? CreateBy { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<DiscountProduct> DiscountProducts { get; set; } =
        new List<DiscountProduct>();

    [ForeignKey("ModifiedById")]
    [InverseProperty("ProductModifiedBies")]
    public virtual User? ModifiedBy { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<ProductAttribute> ProductAttributes { get; set; } =
        new List<ProductAttribute>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductCategory> ProductCategories { get; set; } =
        new List<ProductCategory>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } =
        new List<ProductVariant>();

    [InverseProperty("Product")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
