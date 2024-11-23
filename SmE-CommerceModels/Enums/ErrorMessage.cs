namespace SmE_CommerceModels.Enums
{
    public static class ErrorMessage
    {
        // Common error messages
        public const string InternalServerError = "An unexpected internal server error occurred. Please try again later.";

        // Not found error messages
        public const string NotFound = "not found";
        public const string AddressNotFound = $"Address {NotFound}";
        public const string ProductNotFound = $"Product {NotFound}";
        public const string CategoryNotFound = $"Category {NotFound}";
        public const string UserNotFound = $"User {NotFound}";
        public const string OrderNotFound = $"Order {NotFound}";
        public const string CartNotFound = $"Cart {NotFound}";
        public const string ProductImageNotFound = $"Product image {NotFound}";
        public const string ProductAttributeNotFound = $"Product attribute {NotFound}";

        // Already exists error messages
        public const string AlreadyExists = "already exists";
        public const string EmailAlreadyExists = $"Email {AlreadyExists}";
        public const string PhoneAlreadyExists = $"Phone {AlreadyExists}";
        public const string AddressAlreadyExists = $"Address {AlreadyExists}";
        public const string SlugAlreadyExists = $"Slug {AlreadyExists}";

        // Out of stock error messages
        public const string OutOfStock = "Out of stock";

        // Invalid input error messages
        public const string InvalidPassword = "Invalid password";
        public const string InvalidEmail = "Invalid email";
        public const string InvalidToken = "Token verification failed";
        public const string InvalidInput = "Invalid input";
        public const string InvalidPercentage = "Discount value must be a percentage between 0 and 100";
        public const string InvalidNumber = "Must be a positive number";
        public const string InvalidDate = "Invalid date";
        public const string InvalidQuantity = "Invalid quantity";

        // Not authenticated or authorized
        public const string NotAuthentication = "Not authenticated";
        public const string NotAuthority = "You are not authorized to use this function";
        public const string AccountIsInactive = "Account is Inactive";
        public const string InvalidCredentials = "Invalid credentials";
    }
}
