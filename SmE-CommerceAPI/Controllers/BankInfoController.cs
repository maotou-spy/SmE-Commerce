using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.BankInfo;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/banks")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class BankInfoController (ILogger<AuthController> logger, IBankInfoService bankInfoService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddBankInfoAsync([FromBody] AddBankInfoReqDto req)
    {
        try
        {
            var result = await bankInfoService.AddBankInfoByManagerAsync(req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at create category user: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create category user: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
    
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteBankInfoAsync([FromRoute] Guid id)
    {
        try
        {
            var result = await bankInfoService.DeleteBankInfoAsync(id);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at delete category user: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at delete category user: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBankInfo()
    {
        try
        {
            var result = await bankInfoService.GetBanksInfoAsync();

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get bank info: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get bank info: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}