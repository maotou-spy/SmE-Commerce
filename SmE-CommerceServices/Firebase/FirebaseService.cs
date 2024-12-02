using FirebaseAdmin;
using FirebaseAdmin.Auth;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using ErrorCode = SmE_CommerceModels.Enums.ErrorCode;

namespace SmE_CommerceServices.Firebase;

public class FirebaseService(IUserRepository userRepository) : IFirebaseAuthService
{
    #region Authentication

    public async Task<Return<FirebaseToken>> VerifyTokenAsync(string idToken)
    {
        try
        {
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            return new Return<FirebaseToken>
            {
                Data = decodedToken,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                InternalErrorMessage = null,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            if (ex is FirebaseAuthException)
                return new Return<FirebaseToken>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidToken,
                    InternalErrorMessage = null,
                    TotalRecord = 0,
                };

            return new Return<FirebaseToken>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InvalidToken,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    #endregion
}
