using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

public partial class ProductAttribute
{
    [Key]
    [Column("attributeid")]
    public Guid Attributeid { get; set; }

    [Column("productid")]
    public Guid Productid { get; set; }

    [Column("attributename")]
    [StringLength(100)]
    public string Attributename { get; set; } = null!;

    [Column("attributevalue")]
    [StringLength(255)]
    public string Attributevalue { get; set; } = null!;

    [ForeignKey("Productid")]
    [InverseProperty("ProductAttributes")]
    public virtual Product Product { get; set; } = null!;
}
