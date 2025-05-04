namespace SmE_CommerceModels.RequestDtos.Order;

public class UpdateOrderStatusReqDto
{ 
    public IEnumerable<Guid> OrderIds { get; set; } = [];
    
    public string Status { get; set; }
    
    public string Reason { get; set; }
}