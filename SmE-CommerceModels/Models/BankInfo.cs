using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmE_CommerceModels.Models;

[Table("BankInfo")]
public partial class BankInfo
{
    [Key]
    [Column("bankInfoId")]
    public Guid BankInfoId { get; set; }

    [Column("bankCode")]
    [StringLength(10)]
    public string BankCode { get; set; } = null!;

    [Column("bankName")]
    [StringLength(100)]
    public string BankName { get; set; } = null!;

    [Column("bankLogoUrl")]
    [StringLength(100)]
    public string? BankLogoUrl { get; set; }

    [Column("accountNumber")]
    [StringLength(50)]
    public string AccountNumber { get; set; } = null!;

    [Column("accountHolderName")]
    [StringLength(100)]
    public string AccountHolderName { get; set; } = null!;

    /// <summary>
    /// Values: active, inactive, deleted
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

    [ForeignKey("CreateById")]
    [InverseProperty("BankInfoCreateBies")]
    public virtual User? CreateBy { get; set; }

    [ForeignKey("ModifiedById")]
    [InverseProperty("BankInfoModifiedBies")]
    public virtual User? ModifiedBy { get; set; }
}
