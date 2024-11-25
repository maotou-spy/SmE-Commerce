namespace SmE_CommerceModels.Models;

public class User
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

    public DateTime? CreatedAt { get; set; }

    public Guid? CreateById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public Guid? ModifiedById { get; set; }

    public string? Avatar { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public bool? IsEmailVerified { get; set; }

    public bool? IsPhoneVerified { get; set; }

    public string? ResetPasswordToken { get; set; }

    public DateTime? ResetPasswordExpires { get; set; }

    public virtual ICollection<Address> AddressCreateBies { get; set; } = new List<Address>();

    public virtual ICollection<Address> AddressModifiedBies { get; set; } = new List<Address>();

    public virtual ICollection<Address> AddressUsers { get; set; } = new List<Address>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<BankInfo> BankInfoCreateBies { get; set; } = new List<BankInfo>();

    public virtual ICollection<BankInfo> BankInfoModifiedBies { get; set; } = new List<BankInfo>();

    public virtual ICollection<BlogCategory> BlogCategoryCreateBies { get; set; } = new List<BlogCategory>();

    public virtual ICollection<BlogCategory> BlogCategoryModifiedBies { get; set; } = new List<BlogCategory>();

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<Category> CategoryCreateBies { get; set; } = new List<Category>();

    public virtual ICollection<Category> CategoryModifiedBies { get; set; } = new List<Category>();

    public virtual ICollection<Content> ContentCreateBies { get; set; } = new List<Content>();

    public virtual ICollection<Content> ContentModifiedBies { get; set; } = new List<Content>();

    public virtual User? CreateBy { get; set; }

    public virtual ICollection<DiscountCode> DiscountCodeCreateBies { get; set; } = new List<DiscountCode>();

    public virtual ICollection<DiscountCode> DiscountCodeModifiedBies { get; set; } = new List<DiscountCode>();

    public virtual ICollection<DiscountCode> DiscountCodeUsers { get; set; } = new List<DiscountCode>();

    public virtual ICollection<Discount> DiscountCreateBies { get; set; } = new List<Discount>();

    public virtual ICollection<Discount> DiscountModifiedBies { get; set; } = new List<Discount>();

    public virtual ICollection<User> InverseCreateBy { get; set; } = new List<User>();

    public virtual ICollection<User> InverseModifiedBy { get; set; } = new List<User>();

    public virtual User? ModifiedBy { get; set; }

    public virtual ICollection<Order> OrderCreateBies { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderModifiedBies { get; set; } = new List<Order>();

    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();

    public virtual ICollection<Order> OrderUsers { get; set; } = new List<Order>();

    public virtual ICollection<Payment> PaymentCreateBies { get; set; } = new List<Payment>();

    public virtual ICollection<Payment> PaymentModifiedBies { get; set; } = new List<Payment>();

    public virtual ICollection<Product> ProductCreateBies { get; set; } = new List<Product>();

    public virtual ICollection<Product> ProductModifiedBies { get; set; } = new List<Product>();

    public virtual ICollection<ProductVariant> ProductVariantCreateBies { get; set; } = new List<ProductVariant>();

    public virtual ICollection<ProductVariant> ProductVariantModifiedBies { get; set; } = new List<ProductVariant>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Setting> SettingCreateBies { get; set; } = new List<Setting>();

    public virtual ICollection<Setting> SettingModifiedBies { get; set; } = new List<Setting>();
}