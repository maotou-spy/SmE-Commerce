using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class Payment
{
    public uint PaymentId { get; set; }

    public uint? PaymentMethodId { get; set; }

    public uint? OrderId { get; set; }

    public decimal Amount { get; set; }

    /// <summary>
    /// Values: pending, completed
    /// </summary>
    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public uint? CreatedById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public uint? ModifiedById { get; set; }

    public virtual Order? Order { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; }
}
