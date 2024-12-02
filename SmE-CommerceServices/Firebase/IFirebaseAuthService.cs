using FirebaseAdmin.Auth;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Firebase;

public interface IFirebaseAuthService
{
    Task<Return<FirebaseToken>> VerifyTokenAsync(string idToken);
}
