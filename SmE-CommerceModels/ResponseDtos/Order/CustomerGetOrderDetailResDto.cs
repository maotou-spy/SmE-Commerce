namespace SmE_CommerceModels.ResponseDtos.Order;

public class CustomerGetOrderDetailResDto
{
    public required Guid OrderId { get; set; }
    
    public required string  OrderCode { get; set; }
    
    public required string ReceiverName { get; set; }
    
    public required string ReceiverPhone { get; set; }
    
    public required string AddressFull { get; set; }
    
    public string? ShippingCode { get; set; }
    
    public required decimal TotalAmount { get; set; }
    
    public decimal? ShippingFee { get; set; }
    
    public decimal? DiscountAmount { get; set; }
    
    public int? PointUsed { get; set; }
    
    public required int PointsEarned { get; set; }
    
    public string? Note { get; set; }
    
    public decimal?  SubTotal { get; set; }
    
    public DateTime? EstimatedDeliveryDate { get; set; }
    
    public DateTime? ActualDeliveryDate { get; set; }
    
    public string? CancelReason { get; set; }
    
    public string? ReturnReason { get; set; }
    
    public required string Status { get; set; }
    
    public required List<GetOrderItemResDto> OrderItems { get; set; }
}