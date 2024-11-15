namespace SmE_CommerceModels.ResponseDtos.Category;

public class GetCategoryDetailResDto
{
    public required Guid CategoryId { get; set; }

    public required string CategoryName { get; set; }
    
    public string? Description { get; set; }
    
    public string? CategoryImage { get; set; }
}

public class GetCategoryResDto
{
    public required Guid CategoryId { get; set; }

    public required string CategoryName { get; set; }
}