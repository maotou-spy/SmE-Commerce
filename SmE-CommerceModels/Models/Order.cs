namespace SmE_CommerceModels.Models;

public partial class Order : Common
{
    public Guid OrderId { get; set; }

    public Guid UserId { get; set; }

    public Guid? AddressId { get; set; }

    public string? BOlid { get; set; }

    public decimal TotalAmount { get; set; }

    public Guid? DiscountCodeId { get; set; }

    public string? Reason { get; set; }

    public int PointsEarned { get; set; }

    /// <summary>
    /// Values: pending, processing, completed, cancelled, rejected, returned
    /// </summary>
    public string Status { get; set; } = null!;

    public virtual Address? Address { get; set; }

    public virtual DiscountCode? DiscountCode { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
