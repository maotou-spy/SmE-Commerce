namespace SmE_CommerceModels.ResponseDtos.Category.Manager;

public class ManagerGetCategoryResDto : GetCategoryResDto
{
    public required string Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
