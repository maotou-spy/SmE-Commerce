namespace SmE_CommerceModels.ResponseDtos.Category.Custumer;

public class GetCategoryResDto
{
    public required Guid CategoryId { get; set; }
    
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    
    public required string Slug { get; set; }
}