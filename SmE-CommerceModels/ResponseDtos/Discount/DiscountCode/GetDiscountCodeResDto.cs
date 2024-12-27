namespace SmE_CommerceModels.ResponseDtos.Discount.DiscountCode;

public class GetDiscountCodeResDto
{
    public Guid? CodeId { get; set; }

    public required string DiscountCode { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string? Status { get; set; }
}