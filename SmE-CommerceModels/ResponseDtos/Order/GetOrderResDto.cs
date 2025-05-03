using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.ResponseDtos.Order;

public class GetOrderResDto
{
    public Guid OrderId { get; set; }

    public string? OrderCode { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public string? UserName { get; set; } = string.Empty;

    public string? UserEmail { get; set; } = string.Empty;

    public string? UserPhone { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = OrderStatus.Pending;

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
