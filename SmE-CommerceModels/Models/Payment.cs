using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Models;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid? PaymentMethodId { get; set; }

    public Guid? OrderId { get; set; }

    public decimal Amount { get; set; }

    /// <summary>
    /// Values: pending, paid, completed
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public Guid? CreateById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public Guid? ModifiedById { get; set; }

    public virtual Order? Order { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; }
}
