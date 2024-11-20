namespace SmE_CommerceModels.ResponseDtos.Product.Manager;

public class ManagerGetProductDetailResDto : GetProductDetailsResDto
{
    public DateTime? CreatedAt { get; set; }

    public string?  CreatedBy { get; set; } = null!;

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; } = null!;
}
