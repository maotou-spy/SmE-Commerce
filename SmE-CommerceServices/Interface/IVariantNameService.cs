using SmE_CommerceModels.RequestDtos.VariantName;
using SmE_CommerceModels.ResponseDtos.VariantName;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IVariantNameService
{
    Task<Return<List<ManagerVariantNameResDto>>> GetVariantNamesAsync();

    Task<Return<bool>> BulkVariantNameAsync(List<string> req);

    Task<Return<bool>> UpdateVariantNameAsync(Guid nameId, VariantNameReqDto variantNameReq);

    Task<Return<bool>> DeleteVariantNameAsync(Guid nameId);
}
