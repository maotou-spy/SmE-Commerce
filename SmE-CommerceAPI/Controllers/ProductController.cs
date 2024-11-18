using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Category;
using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceModels.ResponseDtos.Product;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[Route("api/products")]
[Authorize(AuthenticationSchemes = "Defaut")]
public class ProductController(
    ICategoryService categoryService,
    IProductService productService,
    ILogger<AuthController> logger) : ControllerBase
{
    // [HttpGet("active")]
    // public async Task<IActionResult> CustomerGetProductsAsync(string? keyword, string? sortBy,
    //     int pageNumber = PagingEnum.PageNumber, int pageSize = PagingEnum.PageSize)
    // {
    //     try
    //     {
    //         var result = await productService.CustomerGetProductsAsync(keyword, sortBy, pageNumber, pageSize);
    //         if (result.IsSuccess) return StatusCode(200, result);
    //         if (result.InternalErrorMessage is not null)
    //             logger.LogError("Error at get products for customer: {ex}", result.InternalErrorMessage);
    //         return Helper.GetErrorResponse(result.Message);
    //     }
    //     catch (Exception ex)
    //     {
    //         logger.LogInformation("Error at get products for customer: {e}", ex);
    //         return StatusCode(500, new Return<IEnumerable<GetProductsResDto>> { Message = ErrorMessage.InternalServerError });
    //     }
    // }

    [HttpGet("categories/active")]
    public async Task<IActionResult> GetCategoriesForCustomerAsync(string? name, int pageNumber = PagingEnum.PageNumber,
        int pageSize = PagingEnum.PageSize)
    {
        try
        {
            var result = await categoryService.GetCategoriesForCustomerAsync(name, pageNumber, pageSize);
            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get categories for customer: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get categories for customer: {e}", ex);
            return StatusCode(500, new Return<IEnumerable<GetProductsResDto>> { Message = ErrorMessage.InternalServerError });
        }
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategoriesForManagerAsync(string? name,
        int pageNumber = PagingEnum.PageNumber, int pageSize = PagingEnum.PageSize)
    {
        try
        {
            var result = await categoryService.GetCategoriesForManagerAsync(name, pageNumber, pageSize);
            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get categories for manager: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get categories for manager: {e}", ex);
            return StatusCode(500,
                new Return<IEnumerable<GetProductsResDto>> { Message = ErrorMessage.InternalServerError });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddProductAsync([FromBody] AddProductReqDto req)
    {
        try
        {
            if (!ModelState.IsValid) return StatusCode(400, Helper.GetValidationErrors(ModelState));
            var result = await productService.AddProductAsync(req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at create product user: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create product user: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
        }
    }

    [HttpPost("{productId:guid}/attributes")]
    [Authorize]
    public async Task<IActionResult> AddProductAttributeAsync([FromBody] AddProductAttributeReqDto req, Guid productId)
    {
        try
        {
            if (!ModelState.IsValid) return StatusCode(400, Helper.GetValidationErrors(ModelState));
            var result = await productService.AddProductAttributeAsync(productId, req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at create product attribute user: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create product attribute user: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
        }
    }

    [HttpPost("{productId:guid}/images")]
    [Authorize]
    public async Task<IActionResult> AddProductImageAsync([FromBody] AddProductImageReqDto req, Guid productId)
    {
        try
        {
            if (!ModelState.IsValid) return StatusCode(400, Helper.GetValidationErrors(ModelState));
            var result = await productService.AddProductImageAsync(productId, req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at create product image user: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create product image user: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
        }
    }

    [HttpPut("{productId:guid}/categories")]
    [Authorize]
    public async Task<IActionResult> UpdateProductCategoryAsync([FromBody] List<Guid> categoryIds, Guid productId)
    {
        try
        {
            if (!ModelState.IsValid) return StatusCode(400, Helper.GetValidationErrors(ModelState));

            var result = await productService.UpdateProductCategoryAsync(productId, categoryIds);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update product category user: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update product category user: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
        }
    }

    [HttpPut("{productId:guid}/attributes/{attributeId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateProductAttributeAsync([FromBody] AddProductAttributeReqDto req,
        Guid productId, Guid attributeId)
    {
        try
        {
            if (!ModelState.IsValid) return StatusCode(400, Helper.GetValidationErrors(ModelState));

            var result = await productService.UpdateProductAttributeAsync(productId, attributeId, req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update product attribute user: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update product attribute user: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
        }
    }

    [HttpDelete("{productId:guid}/attributes/{attributeId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteProductAttributeAsync(Guid productId, Guid attributeId)
    {
        try
        {
            var result = await productService.DeleteProductAttributeAsync(productId, attributeId);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at delete product attribute user: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at delete product attribute user: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
        }
    }

    [HttpPut("{productId:guid}/images/{imageId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateProductImageAsync([FromBody] AddProductImageReqDto req, Guid productId, Guid imageId)
    {
        try
        {
            if (!ModelState.IsValid) return StatusCode(400, Helper.GetValidationErrors(ModelState));

            var result = await productService.UpdateProductImageAsync(productId, imageId, req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update product image user: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update product image user: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
        }
    }

    [HttpDelete("{productId:guid}/images/{imageId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteProductImageAsync(Guid productId, Guid imageId)
    {
        try
        {
            var result = await productService.DeleteProductImageAsync(productId, imageId);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at delete product image user: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at delete product image user: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
        }
    }

    [HttpPut("{productId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateProductReqDto req, Guid productId)
    {
        try
        {
            if (!ModelState.IsValid) return StatusCode(400, Helper.GetValidationErrors(ModelState));

            var result = await productService.UpdateProductAsync(productId, req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update product user: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update product user: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
        }
    }

    [HttpDelete("{productId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteProductAsync(Guid productId)
    {
        try
        {
            var result = await productService.DeleteProductAsync(productId);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at delete product user: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at delete product user: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
        }
    }

    [HttpGet("{productId:guid}/active")]
    public async Task<IActionResult> CustomerGetProductDetailsAsync(Guid productId)
    {
        try
        {
            var result = await productService.CustomerGetProductDetailsAsync(productId);
            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get product by id: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get product by id: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
        }
    }

    [HttpGet("{productId:guid}")]
    [Authorize]
    public async Task<IActionResult> ManagerGetProductDetailsAsync(Guid productId)
    {
        try
        {
            var result = await productService.ManagerGetProductDetailsAsync(productId);
            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get product by id: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get product by id: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
        }
    }
}
