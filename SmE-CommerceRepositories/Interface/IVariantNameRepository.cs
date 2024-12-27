using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface IVariantNameRepository
{
    Task<Return<List<VariantName>>> GetVariantNamesAsync();

    Task<Return<VariantName>> GetVariantNameByIdAsync(Guid id);

    Task<Return<List<VariantName>>> GetVariantNamesByIdsAsync(List<Guid> ids);

    Task<Return<bool>> BulkCreateVariantNameAsync(List<VariantName> variants);

    Task<Return<bool>> UpdateVariantNameAsync(VariantName variants);

    Task<Return<bool>> DeleteVariantNamesAsync(VariantName variants);
}
