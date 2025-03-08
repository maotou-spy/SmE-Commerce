using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories;

public class BankInforRepository(SmECommerceContext dbContext) : IBankInfoRepository
{
    public async Task<Return<bool>> AddBankInfoByManagerAsync(BankInfo bankInfo)
    {
        try
        {
            await dbContext.BankInfos.AddAsync(bankInfo);
            await dbContext.SaveChangesAsync();

            return new Return<bool>
            {
                IsSuccess = true,
                Data = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                Data = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    public async Task<Return<BankInfo>> GetBankInfoByBankCodeAsync(string code)
    {
        try
        {
            var result = await dbContext
                .BankInfos.Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.BankCode == code);

            return new Return<BankInfo>
            {
                IsSuccess = true,
                Data = result,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception e)
        {
            return new Return<BankInfo>
            {
                IsSuccess = false,
                Data = null,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    public async Task<Return<BankInfo>> GetBankInfoByBankNameAsync(string name)
    {
        try
        {
            var result = await dbContext
                .BankInfos.Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.BankName == name);

            return new Return<BankInfo>
            {
                IsSuccess = true,
                Data = result,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception e)
        {
            return new Return<BankInfo>
            {
                IsSuccess = false,
                Data = null,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    public async Task<Return<BankInfo>> GetBankInfoByAccountNumberAsync(string accountNumber)
    {
        try
        {
            var result = await dbContext
                .BankInfos.Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.AccountNumber == accountNumber);

            return new Return<BankInfo>
            {
                IsSuccess = true,
                Data = result,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception e)
        {
            return new Return<BankInfo>
            {
                IsSuccess = false,
                Data = null,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    public async Task<Return<BankInfo>> GetBankInfoByIdAsync(Guid id)
    {
        try
        {
            var result = await dbContext
                .BankInfos.Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.BankInfoId == id);

            return new Return<BankInfo>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception e)
        {
            return new Return<BankInfo>
            {
                Data = null,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                InternalErrorMessage = e,
            };
        }
    }

    public async Task<Return<IEnumerable<BankInfo>>> GetAllBanksInfoAsync()
    {
        try
        {
            var result = await dbContext
                .BankInfos.Where(x => x.Status != GeneralStatus.Deleted)
                .ToListAsync();

            return new Return<IEnumerable<BankInfo>>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = result.Count,
            };
        }
        catch (Exception e)
        {
            return new Return<IEnumerable<BankInfo>>
            {
                Data = null,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                InternalErrorMessage = e,
            };
        }
    }

    public async Task<Return<bool>> UpdateBankInfoAsync(BankInfo bankInfo)
    {
        try
        {
            dbContext.BankInfos.Update(bankInfo);
            await dbContext.SaveChangesAsync();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                InternalErrorMessage = e,
            };
        }
    }
}
