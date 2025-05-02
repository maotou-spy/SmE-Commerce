using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.ResponseDtos.Product;

public class GetProductsResDto
{
    public Guid ProductId { get; set; }

    public string ProductCode { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; } = 0;

    public int SoldQuantity { get; set; } = 0;

    public string PrimaryImage { get; set; } = null!;

    public bool IsTopSeller { get; set; } = false;

    public decimal? AverageRating { get; set; }

    public string Status { get; set; } = ProductStatus.Active;
}
