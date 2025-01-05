using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Setting;
using SmE_CommerceModels.ResponseDtos.setting;
using SmE_CommerceModels.ResponseDtos.setting.Manager;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface ISettingService
{
    Task<Return<List<SettingResDto>>> GetSettingsAsync();

    Task<Return<SettingResDto>> GetSettingByKeyAsync(string key);

    Task<Return<List<ManagerSettingResDto>>> UpdateSettingAsync(List<SettingReqDto> settings);
}
