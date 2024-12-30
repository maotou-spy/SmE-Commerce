using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceServices.Interface;
using ErrorCode = SmE_CommerceModels.Enums.ErrorCode;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/products")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class ProductController(IProductService productService, ILogger<AuthController> logger)
    : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddProductAsync([FromBody] AddProductReqDto req)
    {
        try
        {
            var result = await productService.AddProductAsync(req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at create product user: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create product user: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPost("{productId:guid}/attributes")]
    [Authorize]
    public async Task<IActionResult> AddProductAttributeAsync(
        [FromBody] AddProductAttributeReqDto req,
        Guid productId
    )
    {
        try
        {
            var result = await productService.AddProductAttributeAsync(productId, req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at create product attribute user: {ex}",
                    result.InternalErrorMessage
                );
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create product attribute user: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPost("{productId:guid}/images")]
    [Authorize]
    public async Task<IActionResult> AddProductImageAsync(
        [FromBody] AddProductImageReqDto req,
        Guid productId
    )
    {
        try
        {
            var result = await productService.AddProductImageAsync(productId, req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at create product image user: {ex}",
                    result.InternalErrorMessage
                );
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create product image user: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("{productId:guid}/categories")]
    [Authorize]
    public async Task<IActionResult> UpdateProductCategoryAsync(
        [FromBody] List<Guid> categoryIds,
        Guid productId
    )
    {
        try
        {
            var result = await productService.UpdateProductCategoryAsync(productId, categoryIds);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at update product category user: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update product category user: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("{productId:guid}/attributes/{attributeId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateProductAttributeAsync(
        [FromBody] AddProductAttributeReqDto req,
        Guid productId,
        Guid attributeId
    )
    {
        try
        {
            var result = await productService.UpdateProductAttributeAsync(
                productId,
                attributeId,
                req
            );

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at update product attribute user: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update product attribute user: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("{productId:guid}/attributes/{attributeId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteProductAttributeAsync(Guid productId, Guid attributeId)
    {
        try
        {
            var result = await productService.DeleteProductAttributeAsync(productId, attributeId);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at delete product attribute user: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at delete product attribute user: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("{productId:guid}/images/{imageId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateProductImageAsync(
        [FromBody] AddProductImageReqDto req,
        Guid productId,
        Guid imageId
    )
    {
        try
        {
            var result = await productService.UpdateProductImageAsync(productId, imageId, req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at update product image user: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update product image user: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("{productId:guid}/images/{imageId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteProductImageAsync(Guid productId, Guid imageId)
    {
        try
        {
            var result = await productService.DeleteProductImageAsync(productId, imageId);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at delete product image user: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at delete product image user: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("{productId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateProductAsync(
        [FromBody] UpdateProductReqDto req,
        Guid productId
    )
    {
        try
        {
            var result = await productService.UpdateProductAsync(productId, req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update product user: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update product user: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("{productId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteProductAsync(Guid productId)
    {
        try
        {
            var result = await productService.DeleteProductAsync(productId);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at delete product user: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at delete product user: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("{productId:guid}/active")]
    [AllowAnonymous]
    public async Task<IActionResult> CustomerGetProductDetailsAsync(Guid productId)
    {
        try
        {
            var result = await productService.CustomerGetProductDetailsAsync(productId);
            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get product by id: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get product by id: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("{productId:guid}")]
    [Authorize]
    public async Task<IActionResult> ManagerGetProductDetailsAsync(Guid productId)
    {
        try
        {
            var result = await productService.ManagerGetProductDetailsAsync(productId);
            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get product by id: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get product by id: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    // Product Variant
    [HttpPost("{productId:guid}/variants")]
    [Authorize]
    public async Task<IActionResult> BulkProductVariantAsync(
        [FromBody] List<AddProductVariantReqDto> req,
        Guid productId
    )
    {
        try
        {
            var result = await productService.BulkProductVariantAsync(productId, req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at create product variant user: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create product variant user: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}
