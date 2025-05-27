using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceServices.Interface;
using ErrorCode = SmE_CommerceModels.Enums.ErrorCode;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class ProductController(IProductService productService, ILogger<AuthController> logger)
    : ControllerBase
{
    [HttpPost("admin/products")]
    [OpenApiOperation("Create Product", "Create Product")]
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
            logger.LogError("Error at create product user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("products")]
    [OpenApiOperation("Get Products", "Get Products")]
    [AllowAnonymous]
    public async Task<IActionResult> CustomerGetProductsAsync(
        [FromQuery] ProductFilterReqDto filter
    )
    {
        try
        {
            var result = await productService.CustomerGetProductsAsync(filter);
            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get products: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at get products: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("admin/products")]
    [OpenApiOperation("Get Products By Manager", "Get Products By Manager")]
    [Authorize]
    public async Task<IActionResult> ManagerGetProductsAsync([FromQuery] ProductFilterReqDto filter)
    {
        try
        {
            var result = await productService.ManagerGetProductsAsync(filter);
            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get products: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at get products: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("admin/products/related/{productId:guid}")]
    [OpenApiOperation("Get Related Products", "Get Related Products")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRelatedProductsAsync(
        Guid productId,
        [FromQuery] int limit = 5
    )
    {
        try
        {
            var result = await productService.GetRelatedProducts(productId, limit);
            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get related products: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at get related products: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPost("admin/products/{productId:guid}/attributes")]
    [OpenApiOperation("Create Product Attribute", "Create Product Attribute")]
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
            logger.LogError("Error at create product attribute user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPost("admin/products/{productId:guid}/images")]
    [OpenApiOperation("Create Product Image", "Create Product Image")]
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
            logger.LogError("Error at create product image user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("admin/products/{productId:guid}/categories")]
    [OpenApiOperation("Update Product Category", "Update Product Category")]
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
            logger.LogError("Error at update product category user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("admin/products/{productId:guid}/attributes/{attributeId:guid}")]
    [OpenApiOperation("Update Product Attribute", "Update Product Attribute")]
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
            logger.LogError("Error at update product attribute user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("admin/products/{productId:guid}/attributes/{attributeId:guid}")]
    [OpenApiOperation("Delete Product Attribute", "Delete Product Attribute")]
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
            logger.LogError("Error at delete product attribute user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("admin/products/{productId:guid}/images/{imageId:guid}")]
    [OpenApiOperation("Update Product Image", "Update Product Image")]
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
            logger.LogError("Error at update product image user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("admin/products/{productId:guid}/images/{imageId:guid}")]
    [OpenApiOperation("Delete Product Image", "Delete Product Image")]
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
            logger.LogError("Error at delete product image user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("admin/products/{productId:guid}")]
    [OpenApiOperation("Update Product", "Update Product")]
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
            logger.LogError("Error at update product user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("admin/products/{productId:guid}")]
    [OpenApiOperation("Delete Product", "Delete Product")]
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
            logger.LogError("Error at delete product user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("products/{productId:guid}")]
    [OpenApiOperation("Product Details", "Product Details")]
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
            logger.LogError("Error at get product by id: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("admin/products/{productId:guid}")]
    [OpenApiOperation("Get Product Details By Manager", "Get Product Details By Manager")]
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
            logger.LogError("Error at get product by id: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    // Product Variant
    [HttpPost("admin/products/{productId:guid}/variants")]
    [OpenApiOperation("Create Product Variant", "Create Product Variant")]
    [Authorize]
    public async Task<IActionResult> AddProductVariantAsync(
        [FromBody] List<ProductVariantReqDto> req,
        Guid productId
    )
    {
        try
        {
            var result = await productService.AddProductVariantAsync(productId, req);

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
            logger.LogError("Error at create product variant user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("admin/products/{productId:guid}/variants/{variantId:guid}")]
    [OpenApiOperation("Update Product Variant", "Update Product Variant")]
    [Authorize]
    public async Task<IActionResult> UpdateProductVariantAsync(
        [FromBody] ProductVariantReqDto req,
        Guid productId,
        Guid variantId
    )
    {
        try
        {
            var result = await productService.UpdateProductVariantAsync(productId, variantId, req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at update product variant user: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at update product variant user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("admin/products/{productId:guid}/variants/{variantId:guid}")]
    [OpenApiOperation("Delete Product Variant", "Delete Product Variant")]
    [Authorize]
    public async Task<IActionResult> DeleteProductVariantAsync(Guid productId, Guid variantId)
    {
        try
        {
            var result = await productService.DeleteProductVariantAsync(productId, variantId);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at delete product variant user: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at delete product variant user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}
