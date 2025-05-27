namespace SmE_CommerceModels.ResponseDtos.Product;

public class GetProductDetailsResDto
{
    public Guid ProductId { get; set; }

    public required string ProductCode { get; set; }

    public string Name { get; set; } = null!;

    public required string PrimaryImage { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int SoldQuantity { get; set; }

    public string Status { get; set; } = null!;

    public bool IsTopSeller { get; set; }

    public bool HasVariant { get; set; } = false;

    public decimal? AverageRating { get; set; }

    public int? TotalRating { get; set; }

    public List<GetProductImageResDto>? Images { get; set; } = [];

    public List<GetProductAttributeResDto> Attributes { get; set; } = [];

    public List<GetProductCategoryResDto> Categories { get; set; } = [];

    public List<GetProductVariantResDto> Variants { get; set; } = [];

    public List<GetProductReviewResDto> Reviews { get; set; } = [];

    public required SeoMetadata SeoMetadata { get; set; }
    
    public IEnumerable<GetRelatedProductResDto> RelatedProducts { get; set; } = new List<GetRelatedProductResDto>();
}
