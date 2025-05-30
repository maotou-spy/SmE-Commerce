﻿namespace SmE_CommerceModels.Enums;

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
    public const string Pending = "Pending"; // Order has been placed but not confirmed by the shop owner
    public const string Stuffing = "Stuffing"; // Order has been confirmed and stuffing by the shop owner
    public const string Shipped = "Shipped"; // Order has been shipped
    public const string Completed = "Completed"; // Order has been delivered
    public const string Cancelled = "Cancelled"; // Order has been canceled by the customer

    public const string Rejected = "Rejected"; // Order has been rejected by the shop owner
    // public const string DeliveryFailed = "DeliveryFailed"; // Delivery has been returned to the sender because cannot be delivered
    // public const string Returned = "Returned"; // Order has been returned by the customer
}

public static class ProductStatus
{
    public const string Active = "Active";
    public const string Inactive = "Inactive";
    public const string OutOfStock = "Out of Stock";
    public const string Deleted = "Deleted";
}
