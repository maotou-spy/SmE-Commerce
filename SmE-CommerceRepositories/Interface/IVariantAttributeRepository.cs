using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface IVariantAttributeRepository
{
    Task<Return<List<VariantAttribute>>> GetVariantAttributes();

    Task<Return<VariantAttribute>> GetVariantAttributeById(Guid id);

    Task<Return<bool>> BulkCreateVariantAttribute(List<VariantAttribute> attributes);

    Task<Return<bool>> UpdateVariantAttribute(VariantAttribute attribute);

    Task<Return<bool>> DeleteVariantAttributes(VariantAttribute attribute);
}
