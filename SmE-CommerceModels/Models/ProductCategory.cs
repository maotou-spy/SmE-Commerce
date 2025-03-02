using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmE_CommerceModels.Models;

public class ProductCategory
{
    [Key]
    [Column("productCategoryId")]
    public Guid ProductCategoryId { get; set; }

    [Column("productId")]
    public Guid ProductId { get; set; }

    [Column("categoryId")]
    public Guid CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("ProductCategories")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductCategories")]
    public virtual Product Product { get; set; } = null!;
}
