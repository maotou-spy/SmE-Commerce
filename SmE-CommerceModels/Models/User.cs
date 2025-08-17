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

    [Column("username")]
    [StringLength(50)]
    public string Username { get; set; } = null!;

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

    [Column("numberOfOrders")]
    public int NumberOfOrders { get; set; } = 0;

    [Column("totalSpent")]
    public decimal TotalSpent { get; set; } = 0;

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
    [StringLength(50)]
    public string? CreateById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    [StringLength(50)]
    public string? ModifiedById { get; set; }

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

    [InverseProperty("User")]
    public virtual ICollection<Address> AddressUsers { get; set; } = new List<Address>();

    [InverseProperty("User")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("User")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [InverseProperty("User")]
    public virtual ICollection<DiscountCode> DiscountCodeUsers { get; set; } =
        new List<DiscountCode>();

    [InverseProperty("User")]
    public virtual ICollection<Order> OrderUsers { get; set; } = new List<Order>();

    [InverseProperty("User")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
