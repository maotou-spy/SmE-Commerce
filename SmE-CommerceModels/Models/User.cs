using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

[Index("Email", Name = "Users_email_key", IsUnique = true)]
[Index("Phone", Name = "users_phone_key", IsUnique = true)]
public class User
{
    [Key]
    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("role")]
    [StringLength(30)]
    public string Role { get; set; } = null!;

    [Column("passwordHash")]
    [StringLength(255)]
    public string? PasswordHash { get; set; }

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column("fullName")]
    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [Column("phone")]
    [StringLength(12)]
    public string? Phone { get; set; }

    [Column("point")]
    public int Point { get; set; }

    /// <summary>
    ///     Values: active, inactive, suspended
    /// </summary>
    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("lastLogin", TypeName = "timestamp without time zone")]
    public DateTime? LastLogin { get; set; }

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("createById")]
    public Guid? CreateById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    public Guid? ModifiedById { get; set; }

    [Column("avatar")]
    [StringLength(255)]
    public string? Avatar { get; set; }

    [Column("dateOfBirth")]
    public DateOnly? DateOfBirth { get; set; }

    [Column("gender")]
    [StringLength(10)]
    public string? Gender { get; set; }

    [Column("isEmailVerified")]
    public bool? IsEmailVerified { get; set; }

    [Column("isPhoneVerified")]
    public bool? IsPhoneVerified { get; set; }

    [Column("resetPasswordToken")]
    [StringLength(255)]
    public string? ResetPasswordToken { get; set; }

    [Column("resetPasswordExpires", TypeName = "timestamp without time zone")]
    public DateTime? ResetPasswordExpires { get; set; }

    [InverseProperty("CreateBy")]
    public virtual ICollection<Address> AddressCreateBies { get; set; } = new List<Address>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<Address> AddressModifiedBies { get; set; } = new List<Address>();

    [InverseProperty("User")]
    public virtual ICollection<Address> AddressUsers { get; set; } = new List<Address>();

    [InverseProperty("User")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("CreateBy")]
    public virtual ICollection<BankInfo> BankInfoCreateBies { get; set; } = new List<BankInfo>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<BankInfo> BankInfoModifiedBies { get; set; } = new List<BankInfo>();

    [InverseProperty("CreateBy")]
    public virtual ICollection<BlogCategory> BlogCategoryCreateBies { get; set; } =
        new List<BlogCategory>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<BlogCategory> BlogCategoryModifiedBies { get; set; } =
        new List<BlogCategory>();

    [InverseProperty("User")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [InverseProperty("CreateBy")]
    public virtual ICollection<Category> CategoryCreateBies { get; set; } = new List<Category>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<Category> CategoryModifiedBies { get; set; } = new List<Category>();

    [InverseProperty("CreateBy")]
    public virtual ICollection<Content> ContentCreateBies { get; set; } = new List<Content>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<Content> ContentModifiedBies { get; set; } = new List<Content>();

    [ForeignKey("CreateById")]
    [InverseProperty("InverseCreateBy")]
    public virtual User? CreateBy { get; set; }

    [InverseProperty("CreateBy")]
    public virtual ICollection<DiscountCode> DiscountCodeCreateBies { get; set; } =
        new List<DiscountCode>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<DiscountCode> DiscountCodeModifiedBies { get; set; } =
        new List<DiscountCode>();

    [InverseProperty("User")]
    public virtual ICollection<DiscountCode> DiscountCodeUsers { get; set; } =
        new List<DiscountCode>();

    [InverseProperty("CreateBy")]
    public virtual ICollection<Discount> DiscountCreateBies { get; set; } = new List<Discount>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<Discount> DiscountModifiedBies { get; set; } = new List<Discount>();

    [InverseProperty("CreateBy")]
    public virtual ICollection<User> InverseCreateBy { get; set; } = new List<User>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<User> InverseModifiedBy { get; set; } = new List<User>();

    [ForeignKey("ModifiedById")]
    [InverseProperty("InverseModifiedBy")]
    public virtual User? ModifiedBy { get; set; }

    [InverseProperty("CreateBy")]
    public virtual ICollection<Order> OrderCreateBies { get; set; } = new List<Order>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<Order> OrderModifiedBies { get; set; } = new List<Order>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } =
        new List<OrderStatusHistory>();

    [InverseProperty("User")]
    public virtual ICollection<Order> OrderUsers { get; set; } = new List<Order>();

    [InverseProperty("CreateBy")]
    public virtual ICollection<Payment> PaymentCreateBies { get; set; } = new List<Payment>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<Payment> PaymentModifiedBies { get; set; } = new List<Payment>();

    [InverseProperty("CreateBy")]
    public virtual ICollection<Product> ProductCreateBies { get; set; } = new List<Product>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<Product> ProductModifiedBies { get; set; } = new List<Product>();

    [InverseProperty("CreateBy")]
    public virtual ICollection<ProductVariant> ProductVariantCreateBies { get; set; } =
        new List<ProductVariant>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<ProductVariant> ProductVariantModifiedBies { get; set; } =
        new List<ProductVariant>();

    [InverseProperty("User")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<Setting> Settings { get; set; } = new List<Setting>();

    [InverseProperty("CreatedBy")]
    public virtual ICollection<VariantName> VariantNameCreatedBies { get; set; } =
        new List<VariantName>();

    [InverseProperty("ModifiedBy")]
    public virtual ICollection<VariantName> VariantNameModifiedBies { get; set; } =
        new List<VariantName>();
}
