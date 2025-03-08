namespace SmE_CommerceModels.ResponseDtos.Product.Manager;

public class ManagerGetProductResDto : GetProductsResDto
{
    public AuditMetadata? AuditMetadata { get; set; }
}
