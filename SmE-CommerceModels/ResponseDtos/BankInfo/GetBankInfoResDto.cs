namespace SmE_CommerceModels.ResponseDtos.BankInfo;

public class GetBankInfoResDto
{
    public required Guid BankInfoId { get; set; }

    public required string BankCode { get; set; }

    public required string BankName { get; set; }

    public string? BankLogoUrl { get; set; }

    public required string Status { get; set; }

    public required string AccountNumber { get; set; }

    public required string AccountHolderName { get; set; }
}
