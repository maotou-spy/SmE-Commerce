namespace SmE_CommerceModels.ResponseDtos.Auth;

public class LoginResDto
{
    public string BearerToken { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
