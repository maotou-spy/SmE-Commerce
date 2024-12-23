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

    public List<GetProductImageResDto>? Images { get; set; } = [];

    public List<GetProductAttributeResDto>? Attributes { get; set; } = [];

    public List<GetProductCategoryResDto>? Categories { get; set; } = [];

    public required SeoMetadata SeoMetadata { get; set; }
}
