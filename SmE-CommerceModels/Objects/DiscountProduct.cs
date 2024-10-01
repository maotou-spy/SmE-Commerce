using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class DiscountProduct
{
    public uint DiscountProductId { get; set; }

    public uint? DiscountId { get; set; }

    public uint? ProductId { get; set; }

    public virtual Discount? Discount { get; set; }

    public virtual Product? Product { get; set; }
}
