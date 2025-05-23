﻿namespace SmE_CommerceModels.ResponseDtos.Order;

public class GetOrderItemResDto
{
    public required Guid OrderItemId { get; set; }
    
    public required Guid ProductId { get; set; }
    
    public Guid? VariantId { get; set; }
    
    public string? VariantName { get; set; }
    
    public required string ProductName { get; set; }
    
    public required decimal Price { get; set; }
    
    public required int Quantity { get; set; }
    
    public string? VariantImage { get; set; }
}