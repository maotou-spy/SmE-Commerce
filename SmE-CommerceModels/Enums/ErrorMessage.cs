namespace SmE_CommerceModels.Enums;

public static class ErrorMessage
{
    public const string InternalServerError = "An unexpected internal server error occurred. Please try again later.";

    public const string NotFound = "not found";
    public const string NotFoundProduct = $"Product {NotFound}";
    public const string NotFoundCategory = $"Category {NotFound}";
    public const string NotFoundDiscount = $"Discount {NotFound}";
    public const string NotFoundUser = $"User {NotFound}";
    public const string NotFoundOrder = $"Order {NotFound}";
    public const string NotFoundAddress = $"Address {NotFound}";
    public const string NotFoundCart = $"Cart {NotFound}";

    public const string AlreadyExists = "already exists";
    public const string EmailAlreadyExists = $"Email {AlreadyExists}";
    public const string PhoneAlreadyExists = $"Phone {AlreadyExists}";
    public const string SlugExisted = $"Slug {AlreadyExists}";
    public const string UserAlreadyExists = $"User {AlreadyExists}";
    public const string DiscountNameAlreadyExists = $"Discount name {AlreadyExists}";
    public const string DiscountCodeAlreadyExists = $"Discount code {AlreadyExists}";
    public const string AddressAlreadyExists = $"Address {AlreadyExists}";

    public const string InvalidPassword = "Invalid password";
    public const string InvalidEmail = "Invalid email";
    public const string InvalidToken = "Token verification failed";
    public const string AccountIsInactive = "Account is Inactive";
    public const string InvalidCredentials = "Invalid credentials";
    public const string InvalidInput = "Invalid input";
    public const string NotAuthentication = "Not authentication";
    public const string NotAuthority = "You are not authority to use this function";
    public const string NoChanges = "No changes";
    public const string ManagerCannotBeBanned = "Manager cannot be banned";
    public const string NotAvailable = "Not available";
    public const string InvalidData = "Invalid data";
    public const string InvalidPercentage = "Discount value must be a percentage between 0 and 100";
    public const string InvalidNumber = "Must be a positive number";
    public const string InvalidDate = "Invalid date";
    public const string InvalidQuantity = "Invalid quantity";
}
