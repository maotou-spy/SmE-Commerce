using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

[Table("OrderStatusHistory")]
public partial class OrderStatusHistory
{
    [Key]
    [Column("historyId")]
    public Guid HistoryId { get; set; }

    [Column("orderId")]
    public Guid OrderId { get; set; }

    [Column("fromStatus")]
    [StringLength(50)]
    public string FromStatus { get; set; } = null!;

    [Column("toStatus")]
    [StringLength(50)]
    public string ToStatus { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    public Guid? ModifiedById { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [ForeignKey("ModifiedById")]
    [InverseProperty("OrderStatusHistories")]
    public virtual User? ModifiedBy { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderStatusHistories")]
    public virtual Order Order { get; set; } = null!;
}
