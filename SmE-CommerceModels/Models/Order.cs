namespace SmE_CommerceModels.Models;

public class Order
{
    public Guid OrderId { get; set; }

    public Guid UserId { get; set; }

    public Guid? AddressId { get; set; }

    public string? ShippingCode { get; set; }

    public decimal TotalAmount { get; set; }

    public Guid? DiscountCodeId { get; set; }

    public int PointsEarned { get; set; }

    /// <summary>
    /// Values: pending, processing, completed, cancelled, rejected, returned
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public Guid? CreateById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public Guid? ModifiedById { get; set; }

    public string? Note { get; set; }

    public decimal? Discountamount { get; set; }

    public string? OrderCode { get; set; }

    public DateTime? EstimatedDeliveryDate { get; set; }

    public DateTime? ActualDeliveryDate { get; set; }

    public decimal? ShippingFee { get; set; }

    public decimal? SubTotal { get; set; }

    public string? CancelReason { get; set; }

    public string? ReturnReason { get; set; }

    public virtual Address? Address { get; set; }

    public virtual User? CreateBy { get; set; }

    public virtual DiscountCode? DiscountCode { get; set; }

    public virtual User? ModifiedBy { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}