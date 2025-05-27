using SmE_CommerceModels.Enums;
using SmE_CommerceModels.ResponseDtos.HomePage;
using SmE_CommerceModels.ResponseDtos.Product;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class HomepageService(IHelperService helperService, IProductRepository productRepository) : IHomepageService
{
    public async Task<Return<HomepageResDto>> GetHomepageDataAsync()
    {
        try
        {
            var rs = new HomepageResDto();
            
            // Get hot products
            var hotProducts = await productRepository.GetHotProductsAsync();
            if (hotProducts is { IsSuccess: true, Data: not null })
            {
                rs.hotProducts = hotProducts.Data.Select(p => new GetRelatedProductResDto
                {
                    ProductId = p.ProductId,
                    ProductCode = p.ProductCode,
                    Name = p.Name,
                    MainImage = p.PrimaryImage,
                    Price = p.Price,
                    SoldQuantity = p.SoldQuantity,
                    IsTopSeller = p.IsTopSeller,
                    Rating = p.AverageRating,
                    Status = p.Status
                }).ToList();
            }
            
            // Get hot reviews
            var hotReviews = await productRepository.GetIsTopReviewAsync();
            if (hotReviews is { IsSuccess: true, Data: not null })
            {
                rs.hotReviews = hotReviews.Data.Select(r => new GetHotReviewResDto
                {
                    ReviewId = r.ReviewId,
                    UserId = r.UserId,
                    FullName = r.User.FullName,
                    IsTop = r.IsTop,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    Status = r.Status
                }).ToList();
            }

            return new Return<HomepageResDto>
            {
                IsSuccess = true,
                Data = rs,
                StatusCode = ErrorCode.Ok
            };
        } catch (Exception ex)
        {
            return new Return<HomepageResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }
}