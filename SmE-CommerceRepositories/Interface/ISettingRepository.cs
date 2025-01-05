using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface ISettingRepository
{
    Task<Return<List<Setting>>> GetSettingsAsync();

    Task<Return<Setting>> GetSettingByKeyAsync(string key);

    Task<Return<List<Setting>>> GetSettingsByIds(List<Guid> settingIds);

    Task<Return<List<Setting>>> UpdateSettingAsync(List<Setting> settings);
}
