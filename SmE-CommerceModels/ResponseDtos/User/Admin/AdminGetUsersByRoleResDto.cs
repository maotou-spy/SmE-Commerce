namespace SmE_CommerceModels.ResponseDtos.User.Admin;

public class AdminGetUsersByRoleResDto
{
    public required string Name { get; set; }

    public string? Avatar { get; set; }

    public string? Address { get; set; }

    public required string Email { get; set; }

    public string? Phone { get; set; }

    public int NumberOfOrders { get; set; } = 0;

    public decimal TotalSpent { get; set; } = 0;

    public bool IsAdmin { get; set; } = false;

    public required string Status { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public Guid? ModifiedBy { get; set; }
}
