using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

public partial class ContentProduct
{
    [Key]
    [Column("contentProductId")]
    public Guid ContentProductId { get; set; }

    [Column("contentId")]
    public Guid ContentId { get; set; }

    [Column("productId")]
    public Guid ProductId { get; set; }

    [ForeignKey("ContentId")]
    [InverseProperty("ContentProducts")]
    public virtual Content Content { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ContentProducts")]
    public virtual Product Product { get; set; } = null!;
}
