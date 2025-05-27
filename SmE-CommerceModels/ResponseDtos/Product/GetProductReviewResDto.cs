namespace SmE_CommerceModels.ResponseDtos.Product;

public class GetProductReviewResDto
{
    public Guid? ProductId { get; set; }
    
    public Guid ReviewId { get; set; }
    
    public Guid? UserId { get; set; }

    public string? Comment { get; set; }

    public decimal Rating { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string? UserImageUrl { get; set; }

    public string? UserName { get; set; } = null!;
}
