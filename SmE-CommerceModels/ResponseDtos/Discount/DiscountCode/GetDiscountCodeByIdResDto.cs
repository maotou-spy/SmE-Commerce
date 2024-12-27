namespace SmE_CommerceModels.ResponseDtos.Discount.DiscountCode;

public class GetDiscountCodeByIdResDto
{
    public Guid CodeId { get; set; }
    
    public required string DiscountName { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime? FromDate { get; set; }
    
    public DateTime? ToDate { get; set; }
    
    public required string Status { get; set; }
    
    public required string Code { get; set; }
}