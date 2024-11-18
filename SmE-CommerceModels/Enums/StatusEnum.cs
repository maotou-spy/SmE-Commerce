namespace SmE_CommerceModels.Enums
{
    public static class GeneralStatus
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Deleted = "Deleted";
    }

    public static class ContentStatus
    {
        public const string Draft = "Draft";
        public const string Pending = "Pending";
        public const string Published = "Published";
        public const string Unpublished = "Unpublished";
        public const string Deleted = "Deleted";
    }
    
    public static class DiscountCodeStatus
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Used = "Used";
        public const string Deleted = "Deleted";
    }

    public static class PaymentStatus
    {
        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string Completed = "Completed";
    }

    public static class UserStatus
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Suspended = "Suspended";
        public const string Deleted = "Deleted";
    }

    public static class OrderStatus
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public const string Rejected = "Rejected";
        public const string Returned = "Returned";
    }

    public static class ProductStatus
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string OutOfStock = "Out of Stock";
        public const string Deleted = "Deleted";
    }
}
