namespace SmE_CommerceModels.ResponseDtos.VariantAttribute;

public class ManagerVariantAttributeResDto
{
    public Guid AttributeId { get; set; }

    public string AttributeName { get; set; } = null!;

    public AuditMetadata? AuditMetadata { get; set; }
}
