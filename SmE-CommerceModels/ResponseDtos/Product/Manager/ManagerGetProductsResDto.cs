namespace SmE_CommerceModels.ResponseDtos.Product.Manager;

public class ManagerGetProductsResDto : GetProductsResDto
{
    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
