using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SmE_CommerceModels.Models;

[Table("BankInfo")]
public class BankInfo
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
    ///     Values: active, inactive, deleted
    /// </summary>
    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("createById")]
    [StringLength(50)]
    public required string CreateById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    [StringLength(50)]
    public string? ModifiedById { get; set; }
}
