using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmE_CommerceModels.Models;

public class ProductAttribute
{
    [Key]
    [Column("attributeId")]
    public Guid AttributeId { get; set; }

    [Column("productId")]
    public Guid ProductId { get; set; }

    [Column("attributeName")]
    [StringLength(100)]
    public string AttributeName { get; set; } = null!;

    [Column("attributeValue")]
    [StringLength(255)]
    public string AttributeValue { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductAttributes")]
    public virtual Product Product { get; set; } = null!;
}
