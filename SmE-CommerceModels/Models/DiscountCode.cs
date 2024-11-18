namespace SmE_CommerceModels.Models;

public partial class DiscountCode : Common
{
    public Guid CodeId { get; set; }

    public Guid DiscountId { get; set; }

    /// <summary>
    /// this code only for this user
    /// </summary>
    public Guid? UserId { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Values: active, inactive, used, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public virtual Discount Discount { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User? User { get; set; }
}
