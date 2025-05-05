namespace SmE_CommerceModels.RequestDtos.Order;

public class ManagerUpdateOrderStatusReqDto
{ 
    public IEnumerable<Guid> OrderIds { get; set; } = [];
    
    public string Status { get; set; }
    
    public string? Reason { get; set; }
}

public class CustomerUpdateOrderStatusReqDto
{
    public Guid OrderId { get; set; }
    
    public string? Reason { get; set; }
}