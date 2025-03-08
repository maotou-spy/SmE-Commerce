using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.BankInfo;
using SmE_CommerceModels.ResponseDtos.BankInfo;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class BankInfoService(IHelperService helperService, IBankInfoRepository bankInfoRepository)
    : IBankInfoService
{
    public async Task<Return<bool>> AddBankInfoByManagerAsync(AddBankInfoReqDto req)
    {
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(
                nameof(RoleEnum.Manager)
            );
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool> { IsSuccess = false, StatusCode = currentUser.StatusCode };

            // Check BankCode isExisted?
            var codeExisted = await bankInfoRepository.GetBankInfoByBankCodeAsync(req.BankCode);
            if (codeExisted is { IsSuccess: true, Data: not null })
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.BankCodeAlreadyExists,
                    Data = false,
                };

            // Check BankName isExisted?
            var nameExisted = await bankInfoRepository.GetBankInfoByBankNameAsync(req.BankName);
            if (nameExisted is { IsSuccess: true, Data: not null })
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.BankNameAlreadyExists,
                    Data = false,
                };

            // Check AccountNumber isExisted?
            var accountNumberExisted = await bankInfoRepository.GetBankInfoByAccountNumberAsync(
                req.AccountNumber
            );
            if (accountNumberExisted is { IsSuccess: true, Data: not null })
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.AccountNumberAlreadyExists,
                    Data = false,
                };

            // Add BankInfo
            var bankInfo = new BankInfo
            {
                BankCode = req.BankCode.ToUpper().Trim(),
                BankName = req.BankName.ToUpper().Trim(),
                BankLogoUrl = req.BankLogoUrl,
                AccountNumber = req.AccountNumber.Trim(),
                AccountHolderName = req.AccountHolderName.ToUpper().Trim(),
                Status = GeneralStatus.Active,
                CreateBy = currentUser.Data,
                CreatedAt = DateTime.Now,
                CreateById = currentUser.Data.UserId,
            };

            var result = await bankInfoRepository.AddBankInfoByManagerAsync(bankInfo);
            if (!result.IsSuccess)
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = result.InternalErrorMessage,
                    StatusCode = result.StatusCode,
                };

            return new Return<bool>
            {
                IsSuccess = result.IsSuccess,
                StatusCode = result.StatusCode,
                Data = result.Data,
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                InternalErrorMessage = e,
                StatusCode = ErrorCode.InternalServerError,
            };
        }
    }

    public async Task<Return<bool>> DeleteBankInfoAsync(Guid id)
    {
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(
                nameof(RoleEnum.Manager)
            );
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool> { IsSuccess = false, StatusCode = currentUser.StatusCode };

            // Check BankInfo isExisted?
            var bankInfo = await bankInfoRepository.GetBankInfoByIdAsync(id);
            if (bankInfo is { IsSuccess: false, Data: null })
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.BankInfoNotFound,
                    Data = false,
                };

            bankInfo.Data!.Status = GeneralStatus.Deleted;
            bankInfo.Data!.ModifiedBy = currentUser.Data;
            bankInfo.Data!.ModifiedAt = DateTime.Now;

            var result = await bankInfoRepository.UpdateBankInfoAsync(bankInfo.Data);
            if (!result.IsSuccess)
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage,
                };

            return new Return<bool>
            {
                IsSuccess = true,
                StatusCode = result.StatusCode,
                Data = result.Data,
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                InternalErrorMessage = e,
            };
        }
    }

    public async Task<Return<IEnumerable<GetBankInfoResDto>>> GetBanksInfoAsync()
    {
        try
        {
            var result = await bankInfoRepository.GetAllBanksInfoAsync();
            if (!result.IsSuccess)
                return new Return<IEnumerable<GetBankInfoResDto>>
                {
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage,
                };

            var banks = result
                .Data!.Select(bankInfo => new GetBankInfoResDto
                {
                    BankInfoId = bankInfo.BankInfoId,
                    BankCode = bankInfo.BankCode,
                    BankName = bankInfo.BankName,
                    BankLogoUrl = bankInfo.BankLogoUrl,
                    AccountNumber = bankInfo.AccountNumber,
                    AccountHolderName = bankInfo.AccountHolderName,
                    Status = bankInfo.Status,
                })
                .ToList();

            return new Return<IEnumerable<GetBankInfoResDto>>
            {
                Data = banks,
                IsSuccess = true,
                StatusCode = result.StatusCode,
                TotalRecord = result.TotalRecord,
            };
        }
        catch (Exception e)
        {
            return new Return<IEnumerable<GetBankInfoResDto>>
            {
                Data = null,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                InternalErrorMessage = e,
            };
        }
    }
}
