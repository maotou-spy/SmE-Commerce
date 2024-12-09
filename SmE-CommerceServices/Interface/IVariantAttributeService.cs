using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.VariantAttribute;
using SmE_CommerceModels.ResponseDtos.VariantAttribute;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IVariantAttributeService
{
    Task<Return<List<ManagerVariantAttributeResDto>>> GetVariantAttributesAsync();

    Task<Return<bool>> BulkCreateVariantAttributeAsync(List<VariantReqDto> variantReqs);

    Task<Return<bool>> UpdateVariantAttributeAsync(Guid variantId, VariantReqDto variantReq);

    Task<Return<bool>> DeleteVariantAttributeAsync(Guid variantId);
}
