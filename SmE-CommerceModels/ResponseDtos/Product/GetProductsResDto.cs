namespace SmE_CommerceModels.ResponseDtos.Product;

public class GetProductsResDto
{
    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; } = 0;

    public string PrimaryImage { get; set; } = null!;

    public bool IsTopSeller { get; set; } = false;

    public List<Dictionary<Guid, string>>? Categories { get; set; }

    public decimal? AverageRating { get; set; }
}
