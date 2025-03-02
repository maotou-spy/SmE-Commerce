using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

public partial class AuditLog
{
    [Key]
    [Column("logId")]
    public Guid LogId { get; set; }

    [Column("userId")]
    public Guid? UserId { get; set; }

    [Column("action")]
    [StringLength(50)]
    public string Action { get; set; } = null!;

    [Column("tableName")]
    [StringLength(50)]
    public string TableName { get; set; } = null!;

    [Column("recordId")]
    public Guid RecordId { get; set; }

    [Column("oldValue", TypeName = "jsonb")]
    public string OldValue { get; set; } = null!;

    [Column("newValue", TypeName = "jsonb")]
    public string NewValue { get; set; } = null!;

    [Column("ipAddress")]
    [StringLength(50)]
    public string IpAddress { get; set; } = null!;

    [Column("userAgent")]
    [StringLength(255)]
    public string? UserAgent { get; set; }

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AuditLogs")]
    public virtual User? User { get; set; }
}
