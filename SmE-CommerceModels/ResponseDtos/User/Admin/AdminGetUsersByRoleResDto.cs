namespace SmE_CommerceModels.ResponseDtos.User;

public class AdminGetUsersByRoleResDto
{
    public required string name { get; set; }

    public string? avatar { get; set; }

    public string? address { get; set; }

    public required string email { get; set; }

    public string? phone { get; set; }

    public int numberOfOrders { get; set; } = 0;

    public decimal totalSpent { get; set; } = 0;

    public required string status { get; set; }

    public DateTime? lastLogin { get; set; }

    public DateTime? createdAt { get; set; }

    public DateTime? modifiedAt { get; set; }

    public Guid? modifiedBy { get; set; }
}
