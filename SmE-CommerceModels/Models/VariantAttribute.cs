namespace SmE_CommerceModels.Models;

public class VariantAttribute
{
    public Guid AttributeId { get; set; }

    public Guid VariantId { get; set; }

    public string AttributeName { get; set; } = null!;

    public string AttributeValue { get; set; } = null!;

    public virtual ProductVariant Variant { get; set; } = null!;
}