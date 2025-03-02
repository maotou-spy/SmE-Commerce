using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmE_CommerceModels.Models;

public class Address
{
    [Key]
    [Column("addressId")]
    public Guid AddressId { get; set; }

    [Column("userId")]
    public Guid? UserId { get; set; }

    [Column("receiverName")]
    [StringLength(100)]
    public string ReceiverName { get; set; } = null!;

    [Column("receiverPhone")]
    [StringLength(12)]
    public string ReceiverPhone { get; set; } = null!;

    [Column("address")]
    [StringLength(100)]
    public string Address1 { get; set; } = null!;

    [Column("ward")]
    [StringLength(100)]
    public string Ward { get; set; } = null!;

    [Column("district")]
    [StringLength(100)]
    public string District { get; set; } = null!;

    [Column("city")]
    [StringLength(100)]
    public string City { get; set; } = null!;

    /// <summary>
    ///     Values: active, inactive, deleted
    /// </summary>
    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("createById")]
    public Guid? CreateById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    public Guid? ModifiedById { get; set; }

    [Column("isDefault")]
    public bool IsDefault { get; set; }

    [ForeignKey("CreateById")]
    [InverseProperty("AddressCreateBies")]
    public virtual User? CreateBy { get; set; }

    [ForeignKey("ModifiedById")]
    [InverseProperty("AddressModifiedBies")]
    public virtual User? ModifiedBy { get; set; }

    [InverseProperty("Address")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("UserId")]
    [InverseProperty("AddressUsers")]
    public virtual User? User { get; set; }
}
