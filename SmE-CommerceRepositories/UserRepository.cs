using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DatabaseContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Objects;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interfacre;

namespace SmE_CommerceRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DefaultdbContext _dbContext;

        public UserRepository(DefaultdbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Return<IEnumerable<User>>> GetAllUsersAsync()
        {
            try
            {
                var result = await _dbContext.Users.ToListAsync();

                return new Return<IEnumerable<User>>
                {
                    Data = result,
                    IsSuccess = true,
                    Message = result.Count > 0 ? SuccessfulMessage.Found : SuccessfulMessage.NotFound,
                    TotalRecord = result.Count
                };
            }
            catch (Exception ex)
            {
                return new Return<IEnumerable<User>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }
    }
}
