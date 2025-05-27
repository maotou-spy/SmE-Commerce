using SmE_CommerceModels.ResponseDtos.Product;

namespace SmE_CommerceModels.ResponseDtos.HomePage;

public class HomepageResDto
{
    // hot products
    public List<GetRelatedProductResDto> hotProducts { get; set; }
    
    // top reviews
    public List<GetHotReviewResDto> hotReviews { get; set; }
    
    // latest blogs
}