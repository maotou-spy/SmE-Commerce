namespace SmE_CommerceModels.ResponseDtos.User
{
    public class GetUserProfileResDto
    {
        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public int? Point { get; set; }

        public string? Gender { get; set; }

        public DateOnly? Dob { get; set; }

        public string? Avatar { get; set; }
    }
}