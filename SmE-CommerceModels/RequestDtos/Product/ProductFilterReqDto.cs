using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.RequestDtos.Product;

public class ProductFilterReqDto
{
    public string SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinRating { get; set; }
    public string Status { get; set; } = ProductStatus.Active;
    public string? SortBy { get; set; } // "price", "averageRating", "soldQuantity"
    public string SortOrder { get; set; } = ProductFilterSortOrder.Ascending;
    public int PageNumber { get; set; } = PagingEnum.PageNumber;
    public int PageSize { get; set; } = PagingEnum.PageSize;
}
