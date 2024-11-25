using System.Transactions;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Address;
using SmE_CommerceModels.ResponseDtos.Address;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class AddressService(IAddressRepository addressRepository, IHelperService helperService) : IAddressService
{
    public async Task<Return<IEnumerable<GetUserAddressesResDto>>> GetUserAddressesAsync(int pageSize, int pageNumber)
    {
        try
        {
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Customer);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
            {
                return new Return<IEnumerable<GetUserAddressesResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentCustomer.StatusCode,
                    TotalRecord = 0
                };
            }

            var result = await addressRepository.GetAddressesByUserIdAsync(currentCustomer.Data.UserId);

            if (!result.IsSuccess || result.Data == null)
            {
                return new Return<IEnumerable<GetUserAddressesResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    TotalRecord = 0
                };
            }

            var addresses = result.Data.Select(address => new GetUserAddressesResDto
            {
                AddressId = address.AddressId,
                ReceiverName = address.ReceiverName,
                ReceiverPhone = address.ReceiverPhone,
                Address = address.Address1,
                Ward = address.Ward,
                District = address.District,
                City = address.City,
                IsDefault = address.IsDefault
            }).ToList();

            return new Return<IEnumerable<GetUserAddressesResDto>>
            {
                Data = addresses,
                IsSuccess = true,
                StatusCode = result.StatusCode,
                TotalRecord = result.TotalRecord
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<GetUserAddressesResDto>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<bool>> AddAddressAsync(AddressReqDto addressReq)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Customer);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentCustomer.StatusCode,
                    TotalRecord = 0
                };
            }

            var isDuplicate = await CheckDuplicateAddressAsync(addressReq, currentCustomer.Data.UserId);
            if (!isDuplicate.IsSuccess || isDuplicate.Data)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = isDuplicate.StatusCode,
                    TotalRecord = 0
                };
            }

            if (addressReq.IsDefault)
            {
                var removeDefault = await RemoveCurrentDefaultAddress(currentCustomer.Data.UserId);
                if (!removeDefault.IsSuccess || !removeDefault.Data)
                {
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = removeDefault.StatusCode,
                    };
                }
            }

            var address = new Address
            {
                ReceiverName = addressReq.ReceiverName.Trim(),
                ReceiverPhone = addressReq.ReceiverPhone.Trim(),
                Address1 = addressReq.Address.Trim(),
                Ward = addressReq.Ward.Trim(),
                District = addressReq.District.Trim(),
                City = addressReq.City.Trim(),
                IsDefault = addressReq.IsDefault,
                UserId = currentCustomer.Data.UserId,
                CreatedAt = DateTime.Now,
                CreateById = currentCustomer.Data.UserId,
                Status = GeneralStatus.Active
            };

            var result = await addressRepository.AddAddressAsync(address);

            if (!result.IsSuccess || !result.Data)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    TotalRecord = 0
                };
            }

            scope.Complete();

            return new Return<bool>
            {
                Data = result.Data,
                IsSuccess = result.IsSuccess,
                StatusCode = result.StatusCode,
                TotalRecord = result.TotalRecord
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<GetUserAddressesResDto>> UpdateAddressAsync(Guid addressId, AddressReqDto addressReq)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Customer);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
            {
                return new Return<GetUserAddressesResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentCustomer.StatusCode
                };
            }

            var existingAddress = await addressRepository.GetAddressByIdAsync(addressId);
            if (existingAddress.Data == null || !existingAddress.IsSuccess)
            {
                return new Return<GetUserAddressesResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = existingAddress.StatusCode
                };
            }

            // Check if there is no changes in the address
            if (!HasAddressChanged(existingAddress.Data, addressReq))
            {
                return new Return<GetUserAddressesResDto>
                {
                    Data = new GetUserAddressesResDto
                    {
                        AddressId = existingAddress.Data.AddressId,
                        ReceiverName = existingAddress.Data.ReceiverName,
                        ReceiverPhone = existingAddress.Data.ReceiverPhone,
                        Address = existingAddress.Data.Address1,
                        Ward = existingAddress.Data.Ward,
                        District = existingAddress.Data.District,
                        City = existingAddress.Data.City,
                        IsDefault = existingAddress.Data.IsDefault
                    },
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok
                };
            }


            var isDuplicate = await CheckDuplicateAddressAsync(addressReq, currentCustomer.Data.UserId, addressId);
            if (!isDuplicate.IsSuccess || isDuplicate.Data)
            {
                return new Return<GetUserAddressesResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = isDuplicate.StatusCode,
                    TotalRecord = 0
                };
            }

            if (addressReq.IsDefault)
            {
                var removeDefault = await RemoveCurrentDefaultAddress(currentCustomer.Data.UserId);
                if (!removeDefault.IsSuccess || !removeDefault.Data)
                {
                    return new Return<GetUserAddressesResDto>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = removeDefault.StatusCode
                    };
                }
            }

            existingAddress.Data.ReceiverName = addressReq.ReceiverName.Trim();
            existingAddress.Data.ReceiverPhone = addressReq.ReceiverPhone.Trim();
            existingAddress.Data.Address1 = addressReq.Address.Trim();
            existingAddress.Data.Ward = addressReq.Ward.Trim();
            existingAddress.Data.District = addressReq.District.Trim();
            existingAddress.Data.City = addressReq.City.Trim();
            existingAddress.Data.IsDefault = addressReq.IsDefault;
            existingAddress.Data.ModifiedAt = DateTime.Now;
            existingAddress.Data.ModifiedById = currentCustomer.Data.UserId;

            var result = await addressRepository.UpdateAddressAsync(existingAddress.Data);

            if (result.Data == null || !result.IsSuccess)
            {
                return new Return<GetUserAddressesResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode
                };
            }

            scope.Complete();

            var addressDto = new GetUserAddressesResDto
            {
                ReceiverName = result.Data.ReceiverName,
                ReceiverPhone = result.Data.ReceiverPhone,
                Address = result.Data.Address1,
                Ward = result.Data.Ward,
                District = result.Data.District,
                City = result.Data.City,
                IsDefault = result.Data.IsDefault
            };

            return new Return<GetUserAddressesResDto>
            {
                Data = addressDto,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<GetUserAddressesResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<bool>> DeleteAddressAsync(Guid id)
    {
        try
        {
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Customer);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentCustomer.StatusCode,
                    TotalRecord = 0
                };
            }

            var result = await addressRepository.DeleteAddressAsync(id);

            return new Return<bool>
            {
                Data = result.Data,
                IsSuccess = result.IsSuccess,
                StatusCode = result.StatusCode,
                TotalRecord = result.TotalRecord
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<bool>> SetDefaultAddressAsync(Guid id)
    {
        try
        {
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Customer);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentCustomer.StatusCode,
                    TotalRecord = 0
                };
            }

            var result = await addressRepository.SetDefaultAddressAsync(id);

            return new Return<bool>
            {
                Data = result.Data,
                IsSuccess = result.IsSuccess,
                StatusCode = result.StatusCode,
                TotalRecord = result.TotalRecord
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }

    private async Task<Return<bool>> RemoveCurrentDefaultAddress(Guid userId)
    {
        try
        {
            var defaultAddress = await addressRepository.GetDefaultAddressAsync(userId);

            if (defaultAddress is not { Data: not null, IsSuccess: true })
                return new Return<bool>
                {
                    Data = true,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok
                };

            defaultAddress.Data.IsDefault = false;
            defaultAddress.Data.ModifiedAt = DateTime.Now;
            defaultAddress.Data.ModifiedById = userId;

            var updateAddress = await addressRepository.UpdateAddressAsync(defaultAddress.Data);

            if (updateAddress.Data == null || !updateAddress.IsSuccess)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = updateAddress.StatusCode
                };
            }

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    private async Task<Return<bool>> CheckDuplicateAddressAsync(AddressReqDto addressReq, Guid userId,
        Guid? addressId = null)
    {
        var existingAddresses = await addressRepository.GetAddressesByUserIdAsync(userId);

        if (!existingAddresses.IsSuccess || existingAddresses.Data == null)
        {
            return new Return<bool> { Data = false, IsSuccess = true, StatusCode = ErrorCode.AddressAlreadyExists };
        }

        var isDuplicate = existingAddresses.Data != null && existingAddresses.Data.Any(addr =>
            addr.AddressId != addressId &&
            addr.Status == GeneralStatus.Active &&
            string.Equals(addr.Address1.Trim(), addressReq.Address.Trim(), StringComparison.OrdinalIgnoreCase) &&
            string.Equals(addr.Ward.Trim(), addressReq.Ward.Trim(), StringComparison.OrdinalIgnoreCase) &&
            string.Equals(addr.District.Trim(), addressReq.District.Trim(), StringComparison.OrdinalIgnoreCase) &&
            string.Equals(addr.City.Trim(), addressReq.City.Trim(), StringComparison.OrdinalIgnoreCase));

        return new Return<bool>
        {
            Data = isDuplicate,
            IsSuccess = true,
            StatusCode = isDuplicate ? ErrorCode.AddressAlreadyExists : ErrorCode.Ok,
        };
    }

    private static bool HasAddressChanged(Address existingAddress, AddressReqDto newAddress)
    {
        return !string.Equals(existingAddress.ReceiverName.Trim(), newAddress.ReceiverName.Trim(),
                   StringComparison.OrdinalIgnoreCase) ||
               !string.Equals(existingAddress.ReceiverPhone.Trim(), newAddress.ReceiverPhone.Trim(),
                   StringComparison.OrdinalIgnoreCase) ||
               !string.Equals(existingAddress.Address1.Trim(), newAddress.Address.Trim(),
                   StringComparison.OrdinalIgnoreCase) ||
               !string.Equals(existingAddress.Ward.Trim(), newAddress.Ward.Trim(),
                   StringComparison.OrdinalIgnoreCase) ||
               !string.Equals(existingAddress.District.Trim(), newAddress.District.Trim(),
                   StringComparison.OrdinalIgnoreCase) ||
               !string.Equals(existingAddress.City.Trim(), newAddress.City.Trim(),
                   StringComparison.OrdinalIgnoreCase) ||
               existingAddress.IsDefault != newAddress.IsDefault;
    }
}
