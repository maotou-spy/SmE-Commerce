namespace SmE_CommerceModels.ResponseDtos;

public class AuditMetadata
{
    public string? CreatedById { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? ModifiedById { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
