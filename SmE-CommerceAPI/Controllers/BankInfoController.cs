﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.BankInfo;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class BankInfoController(ILogger<AuthController> logger, IBankInfoService bankInfoService)
    : ControllerBase
{
    [HttpPost("admin/banks")]
    [OpenApiOperation("Add bank info", "Add bank info")]
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
            logger.LogError("Error at create category user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("admin/{id:guid}")]
    [OpenApiOperation("Delete bank info", "Delete bank info")]
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
            logger.LogError("Error at delete category user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet]
    [OpenApiOperation("Get all bank info", "Get all bank info")]
    [AllowAnonymous]
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
            logger.LogError("Error at get bank info: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}
