namespace SmE_CommerceModels.ResponseDtos.VariantAttribute;

public class ManagerVariantAttributeResDto
{
    public Guid AttributeId { get; set; }

    public string AttributeName { get; set; } = null!;

    public Guid? CreatedById { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? ModifiedById { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
