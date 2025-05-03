using System.ComponentModel.DataAnnotations;
using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.RequestDtos.Order;

public class OrderFilterReqDto
{
    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits.")]
    [RegularExpression(
        @"^0\d{9}$",
        ErrorMessage = "Phone number must start with 0 and be exactly 10 digits."
    )]
    public string? Phone { get; set; }
    
    public string? OrderCode { get; set; }
    
    public DateOnly? FromDate { get; set; }
    
    public DateOnly? ToDate { get; set; }
    
    public string? Status { get; set; }
    
    public string SortOrder { get; set; } = FilterSortOrder.Descending;
    
    public int PageNumber { get; set; } = PagingEnum.PageNumber;
    
    public int PageSize { get; set; } = PagingEnum.PageSize;
}