namespace SmE_CommerceModels.Models;

public class Payment
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

    public virtual User? CreateBy { get; set; }

    public virtual User? ModifiedBy { get; set; }

    public virtual Order? Order { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; }
}