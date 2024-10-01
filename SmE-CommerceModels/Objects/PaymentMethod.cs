using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class PaymentMethod
{
    public uint PaymentMethodId { get; set; }

    public string Name { get; set; } = null!;

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
