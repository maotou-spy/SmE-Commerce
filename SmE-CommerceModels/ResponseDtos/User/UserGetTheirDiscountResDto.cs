namespace SmE_CommerceModels.ResponseDtos.User;

public class UserGetTheirDiscountResDto
{
    public Guid CodeId { get; set; }
    
    public required string DiscountName { get; set; }
}