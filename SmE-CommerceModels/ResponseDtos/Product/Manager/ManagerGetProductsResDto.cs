namespace SmE_CommerceModels.ResponseDtos.Product.Manager;

public class ManagerGetProductsResDto : GetProductsResDto
{
    public AuditMetadata? AuditMetadata { get; set; }
}
