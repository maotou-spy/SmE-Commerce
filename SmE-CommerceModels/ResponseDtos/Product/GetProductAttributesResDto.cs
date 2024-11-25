namespace SmE_CommerceModels.ResponseDtos.Product;

public class GetProductAttributeResDto
{
    public Guid AttributeId { get; set; }
    public required string Name { get; set; }
    public required string Value { get; set; }
}