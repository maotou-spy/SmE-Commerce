using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

public partial class VariantAttribute
{
    [Key]
    [Column("variantId")]
    public Guid VariantId { get; set; }

    [Column("variantName")]
    [StringLength(100)]
    public string VariantName { get; set; } = null!;

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("createdById")]
    public Guid? CreatedById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    public Guid? ModifiedById { get; set; }

    [ForeignKey("CreatedById")]
    [InverseProperty("VariantAttributeCreatedBies")]
    public virtual User? CreatedBy { get; set; }

    [ForeignKey("ModifiedById")]
    [InverseProperty("VariantAttributeModifiedBies")]
    public virtual User? ModifiedBy { get; set; }

    [InverseProperty("Attribute")]
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } =
        new List<ProductVariant>();
}
