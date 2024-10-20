namespace SmE_CommerceModels.Models;

public partial class PaymentMethod
{
    public Guid PaymentMethodId { get; set; }

    public string Name { get; set; } = null!;

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
