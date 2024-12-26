using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.RequestDtos.Discount.DiscountCode;

public class AddDiscountCodeReqDto : UpdateDiscountCodeReqDto
{
    public Guid? UserId { get; set; }
    public string Status { get; set; } = DiscountCodeStatus.Active;
}