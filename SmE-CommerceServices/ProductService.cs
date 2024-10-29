using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;
using SmE_CommerceModels.ResponseDtos.Product;

namespace SmE_CommerceServices
{
    public class ProductService(IProductRepository productRepository, IHelperService helperService) : IProductService
    {
        public async Task<Return<bool>> AddProductAsync(AddProductReqDto req)
        {
            throw new NotImplementedException();
        }

        public async Task<Return<IEnumerable<GetProductsResDto>>> GetProductsForCustomerAsync(string? keyword, string? sortBy, int pageNumber, int pageSize)
        {
            try
            {
                var currentCustomer = await helperService.GetCurrentUserWithRole(RoleEnum.Customer);
                if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                {
                    return new Return<IEnumerable<GetProductsResDto>>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = currentCustomer.Message
                    };
                }

                var result = await productRepository.GetProductsForCustomerAsync(keyword, sortBy, pageNumber, pageSize);
                if (!result.IsSuccess)
                {
                    return new Return<IEnumerable<GetProductsResDto>>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = result.Message
                    };
                }

                return new Return<IEnumerable<GetProductsResDto>>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    Message = result.Message,
                    TotalRecord = result.TotalRecord
                };
            }
            catch (Exception ex)
            {
                return new Return<IEnumerable<GetProductsResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex
                };
            }
        }
    }
}
