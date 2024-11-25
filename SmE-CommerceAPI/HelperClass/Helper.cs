using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceAPI.HelperClass;

public static class Helper
{
    /// <summary>
    /// Return error response from model state.
    /// </summary>
    public static Return<Dictionary<string, List<string>?>> GetValidationErrors(ModelStateDictionary modelState)
    {
        var errors = modelState
            .Where(entry => entry.Value != null && entry.Value.Errors.Any())
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value?.Errors.Select(error => error.ErrorMessage).ToList()
            );

        return new Return<Dictionary<string, List<string>?>>
        {
            ValidationErrors = errors,
            IsSuccess = false,
            StatusCode = ErrorCode.ValidationError
        };
    }

    /// <summary>
    /// Return error response with status code.
    /// </summary>
    public static IActionResult GetErrorResponse(int statusCode)
    {
        var result = new Return<dynamic>
        {
            IsSuccess = false,
            StatusCode = statusCode
        };

        var httpStatusCode = GetHttpStatusCode(statusCode);

        return new ObjectResult(result)
        {
            StatusCode = (int)httpStatusCode
        };
    }

    /// <summary>
    /// Map status code to HTTP status code.
    /// </summary>
    private static HttpStatusCode GetHttpStatusCode(int statusCode)
    {
        return statusCode switch
        {
            // Authentication errors
            ErrorCode.NotAuthority or ErrorCode.AccountIsInactive => HttpStatusCode.Forbidden, // 403
            ErrorCode.InvalidToken or ErrorCode.InvalidCredentials => HttpStatusCode.Unauthorized, // 401

            // Validation errors
            ErrorCode.InvalidPercentage or
            ErrorCode.InvalidDate or
            ErrorCode.InvalidPassword or
            ErrorCode.InvalidEmail => HttpStatusCode.BadRequest, // 400

            // Resource not found
            ErrorCode.ProductNotFound or
            ErrorCode.CategoryNotFound or
            ErrorCode.UserNotFound or
            ErrorCode.OrderNotFound or
            ErrorCode.DiscountNotFound or
            ErrorCode.AddressNotFound or
            ErrorCode.CartNotFound or
            ErrorCode.ProductImageNotFound or
            ErrorCode.ProductAttributeNotFound => HttpStatusCode.NotFound, // 404

            // Conflict (already exists)
            ErrorCode.EmailAlreadyExists or
            ErrorCode.PhoneAlreadyExists or
            ErrorCode.NameAlreadyExists or
            ErrorCode.AddressAlreadyExists or
            ErrorCode.SlugAlreadyExists or
            ErrorCode.DiscountCodeAlreadyExists or
            ErrorCode.UserAlreadyExists => HttpStatusCode.Conflict, // 409

            // Business-specific errors
            ErrorCode.OutOfStock => HttpStatusCode.BadRequest, // 400
            ErrorCode.NotForCustomer => HttpStatusCode.Forbidden, // 403

            // Internal server errors
            ErrorCode.InternalServerError => HttpStatusCode.InternalServerError, // 500

            // Default fallback
            _ => HttpStatusCode.InternalServerError // 500
        };
    }
}
