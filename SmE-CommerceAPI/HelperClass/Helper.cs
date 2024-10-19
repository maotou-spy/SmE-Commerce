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
                Message = ErrorMessage.INVALID_INPUT
            };
        }

        public static IActionResult GetErrorResponse(string error)
        {
            var result = new Return<dynamic> { Message = error };

            return error switch
            {
                ErrorMessage.NOT_AUTHENTICATION => new ObjectResult(result) { StatusCode = 401 },
                ErrorMessage.NOT_AUTHORITY => new ObjectResult(result) { StatusCode = 409 },
                _ => new ObjectResult(result) { StatusCode = 500 },
            };
        }
    }
}
