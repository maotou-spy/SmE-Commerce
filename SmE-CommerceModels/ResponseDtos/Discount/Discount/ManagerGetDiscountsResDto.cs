namespace SmE_CommerceModels.ResponseDtos.Discount.Discount;

public class ManagerGetDiscountsResDto
{
    public required Guid discountId { get; set; }
    
    public required string discountName { get; set; }
    
    public string? description { get; set; }
    
    public required bool isPercentage { get; set; }
    
    public required decimal discountValude { get; set; }
    
    public decimal? minimumOrderAmount { get; set; }
    
    public decimal? maximumDiscount { get; set; }
    
    public DateTime? fromDate { get; set; }
    
    public DateTime? toDate { get; set; }
    
    public required string status { get; set; }
    
    public int? usageCount { get; set; }
    
    public int? minQuantity { get; set; }
    
    public int? maxQuantity { get; set; }
    
    public bool isFirstOrder { get; set; }
}