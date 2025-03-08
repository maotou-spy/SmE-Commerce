using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories;

public class SettingRepository(SmECommerceContext defaultdbContext) : ISettingRepository
{
    public async Task<Return<List<Setting>>> GetSettingsAsync()
    {
        try
        {
            var settings = await defaultdbContext.Settings.ToListAsync();
            return new Return<List<Setting>>
            {
                Data = settings,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = settings.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<Setting>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<List<Setting>>> GetSettingsByIds(List<Guid> settingIds)
    {
        try
        {
            var settings = await defaultdbContext
                .Settings.Where(x => settingIds.Contains(x.SettingId))
                .ToListAsync();
            return new Return<List<Setting>>
            {
                Data = settings,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = settings.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<Setting>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<Setting>> GetSettingByKeyAsync(string key)
    {
        try
        {
            var setting = await defaultdbContext.Settings.FirstOrDefaultAsync(x => x.Key == key);
            return new Return<Setting>
            {
                Data = setting,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<Setting>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<List<Setting>>> UpdateSettingAsync(List<Setting> settings)
    {
        try
        {
            defaultdbContext.Settings.UpdateRange(settings);
            await defaultdbContext.SaveChangesAsync();
            return new Return<List<Setting>>
            {
                Data = settings,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<Setting>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }
}
