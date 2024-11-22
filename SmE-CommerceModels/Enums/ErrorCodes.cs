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
        public const int ProductNotFound = 102;
        public const int CategoryNotFound = 103;
        public const int UserNotFound = 104;
        public const int OrderNotFound = 105;

        // Already exists
        public const int AlreadyExists = 201;
        public const int EmailAlreadyExists = 202;
        public const int PhoneAlreadyExists = 203;

        // Out of stock
        public const int OutOfStock = 301;

        // Internal server error
        public const int InternalServerError = 999;
    }
}
