namespace SmE_CommerceModels.Models;

public partial class BankInfo : Common
{
    public Guid BankInfoId { get; set; }

    public string BankCode { get; set; } = null!;

    public string BankName { get; set; } = null!;

    public string? BankLogoUrl { get; set; }

    public string AccountNumber { get; set; } = null!;

    public string AccountHolderName { get; set; } = null!;

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    public string Status { get; set; } = null!;
}
