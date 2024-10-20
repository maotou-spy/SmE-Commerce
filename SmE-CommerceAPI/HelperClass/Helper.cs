using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceAPI.HelperClass
{
    public class Helper
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
                ErrorMessage.NotAuthority => new ObjectResult(result) { StatusCode = 409 },
                _ => new ObjectResult(result) { StatusCode = 500 },
            };
        }
    }
}
