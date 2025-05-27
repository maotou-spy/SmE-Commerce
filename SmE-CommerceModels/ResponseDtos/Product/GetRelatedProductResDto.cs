namespace SmE_CommerceModels.ResponseDtos.Product;

public class GetRelatedProductResDto
{
    public required Guid ProductId { get; set; }
    
    public required string ProductCode { get; set; }
    
    public string Name { get; set; } = null!;
    
    public required string MainImage { get; set; }
    
    public decimal Price { get; set; }
    
    public int SoldQuantity { get; set; }
    
    public bool IsTopSeller { get; set; }
    
    public decimal? Rating { get; set; }
    
    public required string Status { get; set; }
}