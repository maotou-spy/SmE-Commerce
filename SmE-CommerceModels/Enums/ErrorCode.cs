namespace SmE_CommerceModels.Enums
{
    public static class ErrorCode
    {
        // Success
        public const int Ok = 000;

        // Invalid input
        public const int ValidationError = 001;
        public const int InvalidEmail = 002;
        public const int InvalidPassword = 003;
        public const int InvalidToken = 004;
        public const int InvalidDate = 005;
        public const int InvalidPercentage = 006;
        public const int InvalidNumber = 007;
        public const int InvalidDiscountCode = 008;

        // Not found
        public const int ProductNotFound = 101;
        public const int CategoryNotFound = 102;
        public const int UserNotFound = 103;
        public const int OrderNotFound = 104;
        public const int DiscountNotFound = 105;
        public const int AddressNotFound = 106;
        public const int CartNotFound = 107;
        public const int ProductImageNotFound = 108;
        public const int ProductAttributeNotFound = 109;

        // Already exists
        public const int EmailAlreadyExists = 201;
        public const int PhoneAlreadyExists = 202;
        public const int NameAlreadyExists = 203;
        public const int AddressAlreadyExists = 204;
        public const int SlugAlreadyExists = 205;
        public const int DiscountCodeAlreadyExists = 206;
        public const int UserAlreadyExists = 207;

        // Out of stock
        public const int OutOfStock = 301;

        // Authorization
        public const int NotAuthority = 401;
        public const int AccountIsInactive = 402;
        public const int InvalidCredentials = 403;

        // Not For Customer
        public const int NotForCustomer = 501;

        // Internal server error
        public const int InternalServerError = 999;

    }
}
