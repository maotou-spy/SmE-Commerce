using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

public class Payment
{
    [Key]
    [Column("paymentId")]
    public Guid PaymentId { get; set; }

    [Column("paymentMethodId")]
    public Guid PaymentMethodId { get; set; }

    [Column("orderId")]
    public Guid OrderId { get; set; }

    [Column("amount")]
    [Precision(15, 0)]
    public decimal Amount { get; set; }

    /// <summary>
    ///     Values: pending, paid, completed
    /// </summary>
    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("createById")]
    [StringLength(50)]
    public required string CreateById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    [StringLength(50)]
    public string? ModifiedById { get; set; }

    [Column("description")]
    [StringLength(100)]
    public string Description { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("Payments")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("PaymentMethodId")]
    [InverseProperty("Payments")]
    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}
