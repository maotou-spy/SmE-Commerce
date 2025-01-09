using SmE_CommerceModels.RequestDtos.BankInfo;
using SmE_CommerceModels.ResponseDtos.BankInfo;
using SmE_CommerceModels.ReturnResult;
using Task = DocumentFormat.OpenXml.Office2021.DocumentTasks.Task;

namespace SmE_CommerceServices.Interface;

public interface IBankInfoService
{
    Task<Return<bool>> AddBankInfoByManagerAsync(AddBankInfoReqDto req);
    
    Task<Return<bool>> DeleteBankInfoAsync(Guid id);
    
    Task<Return<IEnumerable<GetBankInfoResDto>>> GetBanksInfoAsync();
}