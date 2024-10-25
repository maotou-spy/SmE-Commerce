using SmE_CommerceModels.RequestDtos.Address;
using SmE_CommerceModels.ResponseDtos.Address;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IAddressService
{
    Task<Return<IEnumerable<GetUserAddressesResDto>>> GetUserAddressesAsync(int pageSize, int pageNumber);
    Task<Return<bool>> AddAddressAsync(AddressReqDto addressReq);
    Task<Return<GetUserAddressesResDto>> UpdateAddressAsync(Guid addressId, AddressReqDto addressReq);
    Task<Return<bool>> DeleteAddressAsync(Guid addressId);
    Task<Return<bool>> SetDefaultAddressAsync(Guid addressId);
}
