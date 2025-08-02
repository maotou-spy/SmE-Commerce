namespace SmE_CommerceModels.Enums;

public static class RoleEnum
{
    public const string Customer = "customer";

    // Administrator: user management, notification management, log management
    public const string Administrator = "administrator";

    // Manager: product management, order management, customer management, discount management
    public const string Manager = "manager";
    public const string Staff = "staff";
}
