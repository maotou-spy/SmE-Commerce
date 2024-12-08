using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.VariantAttribute;
using SmE_CommerceModels.ResponseDtos.VariantAttribute;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IVariantAttributeService
{
    Task<Return<List<ManagerVariantAttributeResDto>>> GetVariantAttributesAsync();

    Task<Return<bool>> BulkCreateVariantAttributeAsync(List<AttributeReqDto> attributeReqs);

    Task<Return<bool>> UpdateVariantAttributeAsync(Guid attributeId, AttributeReqDto attributeReq);

    Task<Return<bool>> DeleteVariantAttributeAsync(Guid id);
}
