using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.RequestDtos.Order;

public class OrderFilterReqDto
{
    public string? SearchTerm { get; set; } // Search by orderCode, Email, phone number

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string? Status { get; set; }

    public string SortOrder { get; set; } = FilterSortOrder.Descending;

    public int PageNumber { get; set; } = PagingEnum.PageNumber;

    public int PageSize { get; set; } = PagingEnum.PageSize;
}
