namespace SmE_CommerceModels.ResponseDtos.Order;

public class ManagerGetOrdersResDto
{
    public Guid OrderId { get; set; }
    
    public Guid UserId { get; set; }
    
    public string UserName { get; set; }
    
    public Guid AddressId { get; set; }
    
    public string addressFull { get; set; }
    
    public string? note { get; set; }
    
    public string status { get; set; }
    
    public List<GetOrderItemResDto> orderItems { get; set; }
}