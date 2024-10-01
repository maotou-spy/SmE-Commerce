using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class OrderItem
{
    public uint OrderItemId { get; set; }

    public uint? OrderId { get; set; }

    public uint? ProductId { get; set; }

    public uint? Quantity { get; set; }

    public decimal Price { get; set; }

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
