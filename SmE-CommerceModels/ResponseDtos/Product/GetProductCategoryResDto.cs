namespace SmE_CommerceModels.ResponseDtos.Product;

public class GetProductCategoryResDto
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = null!;
}
