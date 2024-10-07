namespace SmE_CommerceModels.Models;

public partial class Payment : Common
{
    public Guid PaymentId { get; set; }

    public Guid? PaymentMethodId { get; set; }

    public Guid? OrderId { get; set; }

    public decimal Amount { get; set; }

    /// <summary>
    /// Values: pending, paid, completed
    /// </summary>
    public string Status { get; set; } = null!;

    public virtual Order? Order { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; }
}
