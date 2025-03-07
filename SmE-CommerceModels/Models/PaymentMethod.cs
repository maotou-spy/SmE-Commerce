﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmE_CommerceModels.Models;

public class PaymentMethod
{
    [Key]
    [Column("paymentMethodId")]
    public Guid PaymentMethodId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Values: active, inactive, deleted
    /// </summary>
    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [InverseProperty("PaymentMethod")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
