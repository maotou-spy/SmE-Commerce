﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmE_CommerceModels.Models;

public class ProductImage
{
    [Key]
    [Column("imageId")]
    public Guid ImageId { get; set; }

    [Column("productId")]
    public Guid ProductId { get; set; }

    [Column("url")]
    [StringLength(255)]
    public string Url { get; set; } = null!;

    [Column("altText")]
    [StringLength(255)]
    public string? AltText { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductImages")]
    public virtual Product Product { get; set; } = null!;
}
