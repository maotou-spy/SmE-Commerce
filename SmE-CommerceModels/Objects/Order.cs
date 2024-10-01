using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class Order
{
    public uint OrderId { get; set; }

    public uint UserId { get; set; }

    public uint? AddressId { get; set; }

    public string? ShippingCode { get; set; }

    public decimal TotalAmount { get; set; }

    public uint? DiscountCodeId { get; set; }

    public uint PointsEarned { get; set; }

    public string? Reason { get; set; }

    /// <summary>
    /// Values: pending, processing, completed, cancelled, rejected, returned
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public uint? CreatedById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public uint? ModifiedById { get; set; }

    public virtual Address? Address { get; set; }

    public virtual DiscountCode? DiscountCode { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<OrderPoint> OrderPoints { get; set; } = new List<OrderPoint>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
