namespace SmE_CommerceModels.Enums
{
    public static class ErrorCodes
    {
        // Success
        public const int Ok = 000;

        // Invalid input
        public const int InvalidInput = 001;
        public const int InvalidEmail = 002;
        public const int InvalidPassword = 003;
        public const int InvalidToken = 004;

        // Not found
        public const int NotFound = 101;
        public const int AddressNotFound = 102;
        public const int ProductNotFound = 103;
        public const int CategoryNotFound = 104;
        public const int UserNotFound = 105;
        public const int OrderNotFound = 106;
        public const int CartNotFound = 107;
        public const int ProductImageNotFound = 108;
        public const int ProductAttributeNotFound = 109;

        // Already exists
        public const int AlreadyExists = 201;
        public const int EmailAlreadyExists = 202;
        public const int PhoneAlreadyExists = 203;
        public const int AddressAlreadyExists = 204;
        public const int SlugAlreadyExists = 205;

        // Out of stock
        public const int OutOfStock = 301;

        // Authorization
        public const int NotAuthority = 401;
        public const int AccountIsInactive = 402;
        public const int InvalidCredentials = 403;

        // Internal server error
        public const int InternalServerError = 999;
    }
}
