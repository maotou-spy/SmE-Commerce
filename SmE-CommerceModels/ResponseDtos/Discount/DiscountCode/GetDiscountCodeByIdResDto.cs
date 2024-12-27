namespace SmE_CommerceModels.ResponseDtos.Discount.DiscountCode;

public class GetDiscountCodeByIdResDto : GetDiscountCodeResDto
{
    
    public required string DiscountName { get; set; }
    
    public string? Description { get; set; }
}