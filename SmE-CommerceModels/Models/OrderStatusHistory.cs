namespace SmE_CommerceModels.Models;

public class OrderStatusHistory
{
    public Guid HistoryId { get; set; }

    public Guid OrderId { get; set; }

    public string? FromStatus { get; set; }

    public string ToStatus { get; set; } = null!;

    public DateTime? ChangedAt { get; set; }

    public Guid? ChangedById { get; set; }

    public string? Note { get; set; }

    public virtual User? ChangedBy { get; set; }

    public virtual Order Order { get; set; } = null!;
}