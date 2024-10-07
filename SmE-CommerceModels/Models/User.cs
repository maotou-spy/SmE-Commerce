namespace SmE_CommerceModels.Models;

public partial class User : Common
{
    public Guid UserId { get; set; }

    public string Role { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? Email { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public int? Point { get; set; }

    /// <summary>
    /// Values: active, inactive, suspended
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? LastLogin { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<ChangeLog> ChangeLogs { get; set; } = new List<ChangeLog>();

    public virtual ICollection<DiscountCode> DiscountCodes { get; set; } = new List<DiscountCode>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
