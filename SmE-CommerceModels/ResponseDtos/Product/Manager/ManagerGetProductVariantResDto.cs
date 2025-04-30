using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.ResponseDtos.Product.Manager;

public class ManagerGetProductVariantResDto : GetProductVariantResDto
{
    public int SoldQuantity { get; set; } = 0;

    public AuditMetadata? AuditMetadata { get; set; }
}
