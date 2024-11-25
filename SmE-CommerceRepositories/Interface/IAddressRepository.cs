using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface IAddressRepository
{
    Task<Return<IEnumerable<Address>>> GetAddressesByUserIdAsync(Guid userId);
    Task<Return<Address>> GetAddressByIdAsync(Guid addressId);
    Task<Return<bool>> AddAddressAsync(Address address);
    Task<Return<Address>> UpdateAddressAsync(Address address);
    Task<Return<bool>> DeleteAddressAsync(Guid addressId);
    Task<Return<bool>> SetDefaultAddressAsync(Guid addressId);
    Task<Return<Address>> GetDefaultAddressAsync(Guid userId);
}