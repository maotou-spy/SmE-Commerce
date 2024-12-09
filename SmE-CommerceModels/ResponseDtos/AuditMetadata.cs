namespace SmE_CommerceModels.ResponseDtos;

public class AuditMetadata
{
    public Guid? CreatedById { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? ModifiedById { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
