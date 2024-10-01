using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class CartItem
{
    public uint CartItemId { get; set; }

    public uint? UserId { get; set; }

    public uint? ProductId { get; set; }

    public uint? Quantity { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
