namespace SmE_CommerceModels.ResponseDtos.Category;

public class GetCategoryResDto
{
    public required Guid CategoryId { get; set; }

    public required string CategoryName { get; set; }
}
