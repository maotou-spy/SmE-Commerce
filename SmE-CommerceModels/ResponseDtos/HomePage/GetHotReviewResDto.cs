namespace SmE_CommerceModels.ResponseDtos.HomePage;

public class GetHotReviewResDto
{
    public required Guid ReviewId { get; set; }
    
    public required Guid UserId { get; set; }
    
    public required string FullName { get; set; }
    
    public bool IsTop { get; set; }
    
    public required int Rating { get; set; }
    
    public required string? Comment { get; set; }
    
    public required string Status { get; set; }
}