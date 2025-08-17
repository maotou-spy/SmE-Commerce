using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmE_CommerceModels.Models;

[Table("OrderStatusHistory")]
public class OrderStatusHistory
{
    [Key]
    [Column("historyId")]
    public Guid HistoryId { get; set; }

    [Column("orderId")]
    public Guid OrderId { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    [StringLength(50)]
    public string? ModifiedById { get; set; }

    [Column("reason")]
    [StringLength(500)]
    public string? Reason { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderStatusHistories")]
    public virtual Order Order { get; set; } = null!;
}
