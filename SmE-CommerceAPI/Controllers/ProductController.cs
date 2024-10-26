using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Category;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceServices;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers
{
    [Route("api/products")]
    [Authorize(AuthenticationSchemes = "Defaut")]
    public class ProductController(ICategoryService categoryService, IProductService productService, ILogger<AuthController> logger) : ControllerBase
    {
        [HttpPost("categories")]
        [Authorize]
        public async Task<IActionResult> AddCategoryAsync([FromBody] AddCategoryReqDto req)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(400, Helper.GetValidationErrors(ModelState));
                }
                var result = await categoryService.AddCategoryAsync(req);

                if (result.IsSuccess) return StatusCode(200, result);
                if (result.InternalErrorMessage is not null)
                {
                    logger.LogError("Error at create category user: {ex}", result.InternalErrorMessage);
                }
                return Helper.GetErrorResponse(result.Message);

            }
            catch (Exception ex)
            {
                logger.LogInformation("Error at create category user: {e}", ex);
                return StatusCode(500, new Return<bool> { Message = ErrorMessage.ServerError });
            }
        }
    }
}
