using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using Task = DocumentFormat.OpenXml.Office2021.DocumentTasks.Task;

namespace SmE_CommerceRepositories.Interface;

public interface IBankInfoRepository
{
    Task<Return<bool>> AddBankInfoByManagerAsync(BankInfo bankInfo);
    
    Task<Return<BankInfo>> GetBankInfoByBankCodeAsync (string code);
    
    Task<Return<BankInfo>> GetBankInfoByBankNameAsync (string name);    
    
    Task<Return<BankInfo>> GetBankInfoByAccountNumberAsync (string accountNumber);
    
    Task<Return<BankInfo>> GetBankInfoByIdAsync (Guid id);
    
    Task<Return<IEnumerable<BankInfo>>> GetAllBanksInfoAsync ();
    
    Task<Return<bool>> UpdateBankInfoAsync (BankInfo bankInfo);
}