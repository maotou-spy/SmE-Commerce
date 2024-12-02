using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

[Index("Key", Name = "Settings_key_key", IsUnique = true)]
public partial class Setting
{
    [Key]
    [Column("settingId")]
    public Guid SettingId { get; set; }

    /// <summary>
    /// Values: shopName, address, phone, email, maximumTopReview, privacyPolicy, termsOfService, pointsConversionRate
    /// </summary>
    [Column("key")]
    [StringLength(50)]
    public string Key { get; set; } = null!;

    [Column("value")]
    [StringLength(255)]
    public string Value { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("createById")]
    public Guid? CreateById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    public Guid? ModifiedById { get; set; }

    [ForeignKey("CreateById")]
    [InverseProperty("SettingCreateBies")]
    public virtual User? CreateBy { get; set; }

    [ForeignKey("ModifiedById")]
    [InverseProperty("SettingModifiedBies")]
    public virtual User? ModifiedBy { get; set; }
}
