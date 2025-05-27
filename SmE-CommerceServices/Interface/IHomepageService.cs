using SmE_CommerceModels.ResponseDtos.HomePage;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IHomepageService
{
    Task<Return<HomepageResDto>> GetHomepageDataAsync();
}