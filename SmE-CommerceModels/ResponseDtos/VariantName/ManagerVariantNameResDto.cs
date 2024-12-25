namespace SmE_CommerceModels.ResponseDtos.VariantName;

public class ManagerVariantNameResDto
{
    public Guid AttributeId { get; set; }

    public string AttributeName { get; set; } = null!;

    public AuditMetadata? AuditMetadata { get; set; }
}
