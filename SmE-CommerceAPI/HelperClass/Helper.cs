using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceAPI.HelperClass;

public static class Helper
{
    /// <summary>
    ///     Return error response with status code.
    /// </summary>
    public static IActionResult GetErrorResponse(string statusCode)
    {
        var result = new Return<dynamic> { IsSuccess = false, StatusCode = statusCode };

        var httpStatusCode = GetHttpStatusCode(statusCode);

        return new ObjectResult(result) { StatusCode = (int)httpStatusCode };
    }

    /// <summary>
    ///     Map status code to HTTP status code.
    /// </summary>
    private static HttpStatusCode GetHttpStatusCode(string statusCode)
    {
        return statusCode switch
        {
            // ✅ Success
            ErrorCode.Ok => HttpStatusCode.OK,

            // 🔐 Authorization/Auth errors
            ErrorCode.NotAuthority
            or ErrorCode.AccountIsInactive
            or ErrorCode.NotForCustomer
            or ErrorCode.NotYourAddress => HttpStatusCode.Forbidden, // 403

            ErrorCode.InvalidToken or ErrorCode.InvalidCredentials => HttpStatusCode.Unauthorized, // 401

            // ❌ Bad Request
            ErrorCode.BadRequest
            or ErrorCode.InvalidEmail
            or ErrorCode.InvalidPassword
            or ErrorCode.InvalidDate
            or ErrorCode.InvalidPercentage
            or ErrorCode.InvalidNumber
            or ErrorCode.InvalidDiscountCode
            or ErrorCode.InvalidAmount
            or ErrorCode.InvalidTotalAmount
            or ErrorCode.InvalidPoint
            or ErrorCode.InvalidSubTotal
            or ErrorCode.DiscountCodeExpired
            or ErrorCode.OnlyForTheNewUser
            or ErrorCode.InvalidPointBalance
            or ErrorCode.InvalidPrice
            or ErrorCode.InvalidStockQuantity
            or ErrorCode.InvalidImageUrl
            or ErrorCode.AtLeastTwoProductVariant
            or ErrorCode.InvalidVariantAttributeStructure
            or ErrorCode.InvalidQuantity
            or ErrorCode.OverStockQuantity
            or ErrorCode.OrderAmountTooLow => HttpStatusCode.BadRequest, // 400

            // 🔍 Not Found
            ErrorCode.ProductNotFound
            or ErrorCode.CategoryNotFound
            or ErrorCode.UserNotFound
            or ErrorCode.OrderNotFound
            or ErrorCode.DiscountNotFound
            or ErrorCode.AddressNotFound
            or ErrorCode.CartNotFound
            or ErrorCode.ProductImageNotFound
            or ErrorCode.ProductAttributeNotFound
            or ErrorCode.VariantNameNotFound
            or ErrorCode.ProductVariantNotFound
            or ErrorCode.DiscountCodeNotFound
            or ErrorCode.SettingNotFound
            or ErrorCode.BankInfoNotFound
            or ErrorCode.PaymentMethodNotFound
            or ErrorCode.OrderItemNotFound => HttpStatusCode.NotFound, // 404

            // 🌀 Conflict (already exists)
            ErrorCode.EmailAlreadyExists
            or ErrorCode.PhoneAlreadyExists
            or ErrorCode.NameAlreadyExists
            or ErrorCode.AddressAlreadyExists
            or ErrorCode.SlugAlreadyExists
            or ErrorCode.DiscountCodeAlreadyExists
            or ErrorCode.UserAlreadyExists
            or ErrorCode.CategoryHasProducts
            or ErrorCode.VariantNameAlreadyExists
            or ErrorCode.ProductNameAlreadyExists
            or ErrorCode.ProductVariantAlreadyExists
            or ErrorCode.BankCodeAlreadyExists
            or ErrorCode.BankNameAlreadyExists
            or ErrorCode.AccountNumberAlreadyExists
            or ErrorCode.OutOfStock
            or ErrorCode.VariantNameConflict => HttpStatusCode.Conflict, // 409

            // 🧩 Data inconsistency
            ErrorCode.DataInconsistency => HttpStatusCode.Conflict, // 409

            // 🔧 Internal Error
            ErrorCode.InternalServerError => HttpStatusCode.InternalServerError, // 500

            // 🚫 Default fallback
            _ => HttpStatusCode.InternalServerError,
        };
    }
}
