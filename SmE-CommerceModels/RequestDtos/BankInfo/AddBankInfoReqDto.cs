using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.BankInfo;

public class AddBankInfoReqDto
{
    [StringLength(10)]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "BankCode can only contain letters and digits without spaces or special characters.")]
    [Required]
    public required string BankCode { get; set; }
    
    [StringLength(100)]
    [Required]
    public required string BankName { get; set; }
    
    [StringLength(100)]
    [Url(ErrorMessage = "Please provide a valid URL")]
    public string? BankLogoUrl { get; set; }
    
    [Required]
    [RegularExpression(@"^\d+$", ErrorMessage = "AccountNumber must be numeric.")]
    [StringLength(50)]
    public required string AccountNumber { get; set; }
    
    [StringLength(100)]
    [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "AccountHolderName must only contain letters and spaces.")]
    [Required]
    public required string AccountHolderName { get; set; }
}