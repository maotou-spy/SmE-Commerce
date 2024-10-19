using FirebaseAdmin.Auth;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Firebase.Interface;

namespace SmE_CommerceServices.Firebase
{
    public class FirebaseAuthService : IFirebaseAuthService
    {
        private readonly IUserRepository _userRepository;

        public FirebaseAuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Return<FirebaseToken>> VerifyTokenAsync(string idToken)
        {
            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                return new Return<FirebaseToken>
                {
                    Data = decodedToken,
                    IsSuccess = true,
                    Message = SuccessfulMessage.ValidToken,
                    InternalErrorMessage = null,
                    TotalRecord = 1,
                };
            }
            catch (Exception ex)
            {
                if (ex is FirebaseAuthException)
                {
                    return new Return<FirebaseToken>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = ErrorMessage.InvalidToken,
                        InternalErrorMessage = null,
                        TotalRecord = 0,
                    };
                }

                return new Return<FirebaseToken>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InvalidToken,
                    InternalErrorMessage = ex,
                    TotalRecord = 0,
                };
            }
        }
    }
}
