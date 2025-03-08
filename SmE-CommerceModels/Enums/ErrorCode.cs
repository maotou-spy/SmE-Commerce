namespace SmE_CommerceModels.Enums;

public static class ErrorCode
{
    // Success
    public const string Ok = "000";

    // Invalid input
    public const string BadRequest = "001";
    public const string InvalidEmail = "002";
    public const string InvalidPassword = "003";
    public const string InvalidToken = "004";
    public const string InvalidDate = "005";
    public const string InvalidPercentage = "006";
    public const string InvalidNumber = "007";
    public const string InvalidDiscountCode = "008";
    public const string InvalidAmount = "009";
    public const string InvalidTotalAmount = "010";
    public const string InvalidPoint = "011";
    public const string InvalidSubTotal = "012";
    public const string DiscountCodeExpired = "013";
    public const string OnlyForTheNewUser = "014";
    public const string InvalidPointBalance = "015";
    public const string InvalidPrice = "016";

    // Not found
    public const string ProductNotFound = "101";
    public const string CategoryNotFound = "102";
    public const string UserNotFound = "103";
    public const string OrderNotFound = "104";
    public const string DiscountNotFound = "105";
    public const string AddressNotFound = "106";
    public const string CartNotFound = "107";
    public const string ProductImageNotFound = "108";
    public const string ProductAttributeNotFound = "109";
    public const string VariantNameNotFound = "110";
    public const string ProductVariantNotFound = "111";
    public const string DiscountCodeNotFound = "112";
    public const string SettingNotFound = "113";
    public const string BankInfoNotFound = "114";
    public const string PaymentMethodNotFound = "115";
    public const string OrderItemNotFound = "116";
    public const string InvalidQuantity = "117";

    // Already exists
    public const string EmailAlreadyExists = "201";
    public const string PhoneAlreadyExists = "202";
    public const string NameAlreadyExists = "203";
    public const string AddressAlreadyExists = "204";
    public const string SlugAlreadyExists = "205";
    public const string DiscountCodeAlreadyExists = "206";
    public const string UserAlreadyExists = "207";
    public const string CategoryHasProducts = "208";
    public const string VariantNameAlreadyExists = "209";
    public const string ProductNameAlreadyExists = "210";
    public const string ProductVariantAlreadyExists = "211";
    public const string BankCodeAlreadyExists = "212";
    public const string BankNameAlreadyExists = "213";
    public const string AccountNumberAlreadyExists = "214";

    // Conflict
    public const string OutOfStock = "301";
    public const string ProductImageMinimum = "302";
    public const string OverStockQuantity = "303";
    public const string VariantNameConflict = "304";
    public const string OrderAmountTooLow = "305";

    // Authorization
    public const string NotAuthority = "401";
    public const string AccountIsInactive = "402";
    public const string InvalidCredentials = "403";

    // Not For Customer
    public const string NotForCustomer = "501";
    public const string NotYourAddress = "502";

    // Internal server error
    public const string InternalServerError = "999";
}
