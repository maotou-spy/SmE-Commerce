namespace SmE_CommerceModels.ResponseDtos.Product.Manager;

public class ManagerGetProductDetailResDto : GetProductDetailsResDto
{
    public AuditMetadata? AuditMetadata { get; set; }
}
