using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmE_CommerceModels.Models;

public partial class ProductAttribute
{
    [Key]
    [Column("attributeid")]
    public Guid AttributeId { get; set; }

    [Column("productid")]
    public Guid ProductId { get; set; }

    [Column("attributename")]
    [StringLength(100)]
    public string AttributeName { get; set; } = null!;

    [Column("attributevalue")]
    [StringLength(255)]
    public string AttributeValue { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductAttributes")]
    public virtual Product Product { get; set; } = null!;
}
