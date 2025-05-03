namespace SmE_CommerceModels.ResponseDtos.Order;

public class GetOrderDetailsResDto
{
    public Guid OrderId { get; set; }
    
    public string? OrderCode { get; set; }
    
    public Guid UserId { get; set; }
    
    public string? FullName { get; set; }
    
    public Guid AddressId { get; set; }
    
    public string? AddressFull { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public Guid? DiscountCodeId { get; set; }
    
    public string? DiscountCode { get; set; }
    
    public decimal? DiscountAmount { get; set; }
    
    public int PointsEarned { get; set; }
    
    public int PointsUsed { get; set; }
    public string? Status { get; set; }
    
    public DateTime? CreateAt { get; set; }
    
    public Guid? CreatedBy { get; set; }
    
    public string? CreatedByUserName { get; set; }
    
    public DateTime? ModifiedAt { get; set; }
    
    public Guid ModifiedBy { get; set; }
    
    public string? ModifiedByUserName { get; set; }
    
    public string? Note { get; set; }
    
    public decimal? ShippingFee { get; set; }
    
    public decimal? SubTotal { get; set; }
    
    public List<GetOrderItemResDto>? OrderItems { get; set; }
}