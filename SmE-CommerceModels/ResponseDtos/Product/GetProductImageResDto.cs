namespace SmE_CommerceModels.ResponseDtos.Product;

public class GetProductImageResDto
{
    public Guid ImageId { get; set; }
    public required string Url { get; set; }
    public string? AltText { get; set; }
}
