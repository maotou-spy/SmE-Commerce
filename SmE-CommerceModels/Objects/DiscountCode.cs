using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class DiscountCode
{
    public uint CodeId { get; set; }

    public uint? DiscountId { get; set; }

    /// <summary>
    /// this code only for this user
    /// </summary>
    public uint? UserId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    /// <summary>
    /// Values: active, inactive, used, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public uint? CreatedById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public uint? ModifiedById { get; set; }

    public virtual Discount? Discount { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
