using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceAPI.HelperClass
{
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
            var result = new Return<dynamic> { Message = error };

            return error switch
            {
                ErrorMessage.NotAuthentication => new ObjectResult(result) { StatusCode = 401 },
                ErrorMessage.NotAuthority => new ObjectResult(result) { StatusCode = 403 },
                ErrorMessage.Duplicated => new ObjectResult(result) { StatusCode = 409 },
                ErrorMessage.InvalidInput => new ObjectResult(result) { StatusCode = 400 },
                ErrorMessage.InvalidPercentage => new ObjectResult(result) { StatusCode = 400 },
                ErrorMessage.InvalidDate => new ObjectResult(result) { StatusCode = 400 },
                ErrorMessage.NotFound => new ObjectResult(result) { StatusCode = 404 },
                ErrorMessage.InvalidPassword => new ObjectResult(result) { StatusCode = 400 },
                ErrorMessage.InvalidEmail => new ObjectResult(result) { StatusCode = 400 },
                ErrorMessage.InvalidToken => new ObjectResult(result) { StatusCode = 401 },
                ErrorMessage.InvalidCredentials => new ObjectResult(result) { StatusCode = 401 },
                ErrorMessage.ManagerCannotBeBanned => new ObjectResult(result) { StatusCode = 403 },
                ErrorMessage.NotAvailable => new ObjectResult(result) { StatusCode = 404 },
                ErrorMessage.UserAlreadyExists => new ObjectResult(result) { StatusCode = 409 },
                ErrorMessage.EmailAlreadyExists => new ObjectResult(result) { StatusCode = 409 },
                ErrorMessage.PhoneAlreadyExists => new ObjectResult(result) { StatusCode = 409 },
                ErrorMessage.AccountIsInactive => new ObjectResult(result) { StatusCode = 401 },
                _ => new ObjectResult(result) { StatusCode = 500 },
            };
        }
    }
}
