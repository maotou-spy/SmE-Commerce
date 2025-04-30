namespace SmE_CommerceModels.ResponseDtos.Product;

public class GetVariantAttributeResDto
{
    public Guid VariantNameId { get; set; }
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
}
