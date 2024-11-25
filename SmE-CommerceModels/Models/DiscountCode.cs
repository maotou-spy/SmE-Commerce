namespace SmE_CommerceModels.Models;

public class DiscountCode
{
    public Guid CodeId { get; set; }

    public Guid DiscountId { get; set; }

    public required string Code { get; set; }

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

    public DateTime? CreatedAt { get; set; }

    public Guid? CreateById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public Guid? ModifiedById { get; set; }

    public virtual User? CreateBy { get; set; }

    public virtual Discount Discount { get; set; } = null!;

    public virtual User? ModifiedBy { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User? User { get; set; }
}