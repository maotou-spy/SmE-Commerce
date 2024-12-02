using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

public partial class Payment
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
    /// Values: pending, paid, completed
    /// </summary>
    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("createById")]
    public Guid? CreateById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    public Guid? ModifiedById { get; set; }

    [ForeignKey("CreateById")]
    [InverseProperty("PaymentCreateBies")]
    public virtual User? CreateBy { get; set; }

    [ForeignKey("ModifiedById")]
    [InverseProperty("PaymentModifiedBies")]
    public virtual User? ModifiedBy { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("Payments")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("PaymentMethodId")]
    [InverseProperty("Payments")]
    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}
