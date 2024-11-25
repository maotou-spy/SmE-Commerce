using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceAPI.HelperClass;

public static class Helper
{
    public static Return<Dictionary<string, List<string>?>> GetValidationErrors(ModelStateDictionary modelState)
    {
        var errors = modelState.ToDictionary(
            entry => entry.Key,
            entry => entry.Value?.Errors.Select(error => error.ErrorMessage).ToList()
        );

        return new Return<Dictionary<string, List<string>?>>
        {
            Data = errors,
            IsSuccess = false,
            Message = ErrorMessage.InvalidInput
        };
    }

    public static IActionResult GetErrorResponse(string error)
    {
        var result = new Return<dynamic>
        {
            Message = error
        };

        var statusCode = GetStatusCode(error);
        return new ObjectResult(result)
        {
            StatusCode = statusCode
        };
    }

    private static int GetStatusCode(string error)
        => error switch
        {
            ErrorMessage.NotAuthentication => 401,
            ErrorMessage.NotAuthority => 403,
            ErrorMessage.InvalidInput => 400,
            ErrorMessage.InvalidPercentage => 400,
            ErrorMessage.InvalidDate => 400,
            not null when error.Contains(ErrorMessage.NotFound) => 404,
            not null when error.Contains(ErrorMessage.AlreadyExists) => 409,
            ErrorMessage.InvalidPassword => 400,
            ErrorMessage.InvalidEmail => 400,
            ErrorMessage.InvalidToken => 401,
            ErrorMessage.InvalidCredentials => 401,
            ErrorMessage.AccountIsInactive => 403,
            ErrorMessage.OutOfStock => 400,
            _ => 500
        };
}