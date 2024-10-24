using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class AddressService(IAddressRepository addressRepository, IHelperService helperService) : IAddressService
{

}
