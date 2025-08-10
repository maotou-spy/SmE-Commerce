namespace SmE_CommerceModels.ResponseDtos.User;

public class GetUserProfileResDto
{
    public string Email { get; set; }

    public string FullName { get; set; }

    public string? Phone { get; set; }

    public int Point { get; set; } = 0;

    public string? Gender { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Avatar { get; set; }

    public bool IsPhoneVerified { get; set; }

    public bool IsEmailVerified { get; set; }
    
    public string? Address { get; set; }
}
