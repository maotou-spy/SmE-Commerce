using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Setting;
using SmE_CommerceModels.ResponseDtos;
using SmE_CommerceModels.ResponseDtos.setting;
using SmE_CommerceModels.ResponseDtos.setting.Manager;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class SettingService(IHelperService helperService, ISettingRepository settingRepository)
    : ISettingService
{
    public async Task<Return<List<SettingResDto>>> GetSettingsAsync()
    {
        try
        {
            var settings = await settingRepository.GetSettingsAsync();
            if (settings.IsSuccess == false)
                return new Return<List<SettingResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = settings.StatusCode,
                    InternalErrorMessage = settings.InternalErrorMessage,
                };

            return new Return<List<SettingResDto>>
            {
                Data = settings
                    .Data?.Select(x => new SettingResDto
                    {
                        SettingId = x.SettingId,
                        Key = x.Key,
                        Value = x.Value,
                        Description = x.Description,
                    })
                    .ToList(),
                IsSuccess = settings.IsSuccess,
                StatusCode = settings.StatusCode,
                TotalRecord = settings.TotalRecord,
                InternalErrorMessage = settings.InternalErrorMessage,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<SettingResDto>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<SettingResDto>> GetSettingByKeyAsync(string key)
    {
        try
        {
            var setting = await settingRepository.GetSettingByKeyAsync(key);
            if (setting.IsSuccess == false)
                return new Return<SettingResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = setting.StatusCode,
                    InternalErrorMessage = setting.InternalErrorMessage,
                };

            if (setting.Data == null)
                return new Return<SettingResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.SettingNotFound,
                };

            return new Return<SettingResDto>
            {
                Data = new SettingResDto
                {
                    SettingId = setting.Data.SettingId,
                    Key = setting.Data.Key,
                    Value = setting.Data.Value,
                    Description = setting.Data.Description,
                },
                IsSuccess = setting.IsSuccess,
                StatusCode = setting.StatusCode,
                TotalRecord = 1,
                InternalErrorMessage = setting.InternalErrorMessage,
            };
        }
        catch (Exception ex)
        {
            return new Return<SettingResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<List<ManagerSettingResDto>>> UpdateSettingAsync(
        List<SettingReqDto> settings
    )
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<List<ManagerSettingResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var existedSettings = await settingRepository.GetSettingsByIds(
                settings.Select(x => x.SettingId).ToList()
            );
            if (existedSettings.IsSuccess == false)
                return new Return<List<ManagerSettingResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = existedSettings.StatusCode,
                    InternalErrorMessage = existedSettings.InternalErrorMessage,
                };

            if (existedSettings.Data?.Count == 0)
                return new Return<List<ManagerSettingResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.SettingNotFound,
                };

            var settingList = settings
                .Where(inputSetting =>
                    existedSettings.Data != null
                    && existedSettings.Data.Any(existedSetting =>
                        existedSetting.SettingId == inputSetting.SettingId
                    )
                )
                .Select(inputSetting =>
                {
                    var existedSetting = existedSettings.Data?.FirstOrDefault(existed =>
                        existed.SettingId == inputSetting.SettingId
                    );
                    if (existedSetting == null)
                        return null;

                    // Update setting value if it is different
                    if (inputSetting.Value != existedSetting.Value)
                        existedSetting.Value = inputSetting.Value;

                    existedSetting.Description =
                        inputSetting.Description ?? existedSetting.Description;
                    existedSetting.ModifiedById = currentUser.Data.UserId;
                    existedSetting.ModifiedAt = DateTime.Now;

                    return existedSetting;
                })
                .Where(updatedSetting => updatedSetting != null)
                .ToList();

            var settingResult = await settingRepository.UpdateSettingAsync(settingList!);
            if (settingResult.IsSuccess == false)
                return new Return<List<ManagerSettingResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = settingResult.StatusCode,
                    InternalErrorMessage = settingResult.InternalErrorMessage,
                };

            return new Return<List<ManagerSettingResDto>>
            {
                Data = settingResult
                    .Data?.Select(x => new ManagerSettingResDto
                    {
                        SettingId = x.SettingId,
                        Key = x.Key,
                        Value = x.Value,
                        Description = x.Description,
                        AuditMetadata = new AuditMetadata
                        {
                            ModifiedById = x.ModifiedById,
                            ModifiedBy = x.ModifiedBy?.FullName,
                            ModifiedAt = x.ModifiedAt,
                        },
                    })
                    .ToList(),
                IsSuccess = settingResult.IsSuccess,
                StatusCode = settingResult.StatusCode,
                TotalRecord = settingResult.Data?.Count ?? 0,
                InternalErrorMessage = settingResult.InternalErrorMessage,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<ManagerSettingResDto>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }
}
