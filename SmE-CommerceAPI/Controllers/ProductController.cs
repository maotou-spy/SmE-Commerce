using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Category;
using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceModels.ResponseDtos.Product;
using SmE_CommerceModels.ReturnResult;
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

        [HttpGet("active")]
        public async Task<IActionResult> GetProductsForCustomerAsync(string? keyword, string? sortBy, int pageNumber = PagingEnum.PageNumber, int pageSize = PagingEnum.PageSize)
        {
            try
            {
                var result = await productService.GetProductsForCustomerAsync(keyword, sortBy, pageNumber, pageSize);
                if (result.IsSuccess) return StatusCode(200, result);
                if (result.InternalErrorMessage is not null)
                {
                    logger.LogError("Error at get products for customer: {ex}", result.InternalErrorMessage);
                }
                return Helper.GetErrorResponse(result.Message);
            }
            catch (Exception ex)
            {
                logger.LogInformation("Error at get products for customer: {e}", ex);
                return StatusCode(500, new Return<IEnumerable<GetProductsResDto>> { Message = ErrorMessage.ServerError });
            }
        }

        [HttpGet("categories/active")]
        public async Task<IActionResult> GetCategoriesForCustomerAsync(string? name, int pageNumber = PagingEnum.PageNumber, int pageSize = PagingEnum.PageSize)
        {
            try
            {
                var result = await categoryService.GetCategoriesForCustomerAsync(name, pageNumber, pageSize);
                if (result.IsSuccess) return StatusCode(200, result);
                if (result.InternalErrorMessage is not null)
                {
                    logger.LogError("Error at get categories for customer: {ex}", result.InternalErrorMessage);
                }
                return Helper.GetErrorResponse(result.Message);
            }
            catch (Exception ex)
            {
                logger.LogInformation("Error at get categories for customer: {e}", ex);
                return StatusCode(500, new Return<IEnumerable<GetProductsResDto>> { Message = ErrorMessage.ServerError });
            }
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategoriesForManagerAsync(string? name, int pageNumber = PagingEnum.PageNumber, int pageSize = PagingEnum.PageSize)
        {
            try
            {
                var result = await categoryService.GetCategoriesForManagerAsync(name, pageNumber, pageSize);
                if (result.IsSuccess) return StatusCode(200, result);
                if (result.InternalErrorMessage is not null)
                {
                    logger.LogError("Error at get categories for manager: {ex}", result.InternalErrorMessage);
                }
                return Helper.GetErrorResponse(result.Message);
            }
            catch (Exception ex)
            {
                logger.LogInformation("Error at get categories for manager: {e}", ex);
                return StatusCode(500, new Return<IEnumerable<GetProductsResDto>> { Message = ErrorMessage.ServerError });

                [HttpPost]
                [Authorize]
                public async Task<IActionResult> AddProductAsync([FromBody] AddProductReqDto req)
                {
                    try
                    {
                        if (!ModelState.IsValid)
                        {
                            return StatusCode(400, Helper.GetValidationErrors(ModelState));
                        }
                        var result = await productService.AddProductAsync(req);

                        if (result.IsSuccess) return StatusCode(200, result);
                        if (result.InternalErrorMessage is not null)
                        {
                            logger.LogError("Error at create product user: {ex}", result.InternalErrorMessage);
                        }
                        return Helper.GetErrorResponse(result.Message);

                    }
                    catch (Exception ex)
                    {
                        logger.LogInformation("Error at create product user: {e}", ex);
                        return StatusCode(500, new Return<bool> { Message = ErrorMessage.ServerError });
                    }
                }

                [HttpPost("{productId:guid}/attributes")]
                [Authorize]
                public async Task<IActionResult> AddProductAttributeAsync([FromBody] AddProductAttributeReqDto req, Guid productId)
                {
                    try
                    {
                        if (!ModelState.IsValid)
                        {
                            return StatusCode(400, Helper.GetValidationErrors(ModelState));
                        }
                        var result = await productService.AddProductAttributeAsync(productId, req);

                        if (result.IsSuccess) return StatusCode(200, result);
                        if (result.InternalErrorMessage is not null)
                        {
                            logger.LogError("Error at create product attribute user: {ex}", result.InternalErrorMessage);
                        }
                        return Helper.GetErrorResponse(result.Message);

                    }
                    catch (Exception ex)
                    {
                        logger.LogInformation("Error at create product attribute user: {e}", ex);
                        return StatusCode(500, new Return<bool> { Message = ErrorMessage.ServerError });
                    }
                }

                [HttpPost("{productId:guid}/images")]
                [Authorize]
                public async Task<IActionResult> AddProductImageAsync([FromBody] AddProductImageReqDto req, Guid productId)
                {
                    try
                    {
                        if (!ModelState.IsValid)
                        {
                            return StatusCode(400, Helper.GetValidationErrors(ModelState));
                        }
                        var result = await productService.AddProductImageAsync(productId, req);

                        if (result.IsSuccess) return StatusCode(200, result);
                        if (result.InternalErrorMessage is not null)
                        {
                            logger.LogError("Error at create product image user: {ex}", result.InternalErrorMessage);
                        }
                        return Helper.GetErrorResponse(result.Message);

                    }
                    catch (Exception ex)
                    {
                        logger.LogInformation("Error at create product image user: {e}", ex);
                        return StatusCode(500, new Return<bool> { Message = ErrorMessage.ServerError });
                    }
                }

                [HttpPut("{productId:guid}/categories")]
                [Authorize]
                public async Task<IActionResult> UpdateProductCategoryAsync([FromBody] List<Guid> categoryIds, Guid productId)
                {
                    try
                    {
                        if (!ModelState.IsValid)
                        {
                            return StatusCode(400, Helper.GetValidationErrors(ModelState));
                        }

                        var result = await productService.UpdateProductCategoryAsync(productId, categoryIds);

                        if (result.IsSuccess) return StatusCode(200, result);
                        if (result.InternalErrorMessage is not null)
                        {
                            logger.LogError("Error at update product category user: {ex}", result.InternalErrorMessage);
                        }

                        return Helper.GetErrorResponse(result.Message);

                    }
                    catch (Exception ex)
                    {
                        logger.LogInformation("Error at update product category user: {e}", ex);
                        return StatusCode(500, new Return<bool> { Message = ErrorMessage.ServerError });
                    }
                }

                [HttpPut("{productId:guid}/attributes/{attributeId:guid}")]
                [Authorize]
                public async Task<IActionResult> UpdateProductAttributeAsync([FromBody] AddProductAttributeReqDto req, Guid productId, Guid attributeId)
                {
                    try
                    {
                        if (!ModelState.IsValid)
                        {
                            return StatusCode(400, Helper.GetValidationErrors(ModelState));
                        }

                        var result = await productService.UpdateProductAttributeAsync(productId, attributeId, req);

                        if (result.IsSuccess) return StatusCode(200, result);
                        if (result.InternalErrorMessage is not null)
                        {
                            logger.LogError("Error at update product attribute user: {ex}", result.InternalErrorMessage);
                        }

                        return Helper.GetErrorResponse(result.Message);

                    }
                    catch (Exception ex)
                    {
                        logger.LogInformation("Error at update product attribute user: {e}", ex);
                        return StatusCode(500, new Return<bool> { Message = ErrorMessage.ServerError });
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
                        {
                            logger.LogError("Error at delete product attribute user: {ex}", result.InternalErrorMessage);
                        }

                        return Helper.GetErrorResponse(result.Message);

                    }
                    catch (Exception ex)
                    {
                        logger.LogInformation("Error at delete product attribute user: {e}", ex);
                        return StatusCode(500, new Return<bool> { Message = ErrorMessage.ServerError });
                    }
                }
            }
        }
