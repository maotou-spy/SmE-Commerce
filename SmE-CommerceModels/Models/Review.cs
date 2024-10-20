namespace SmE_CommerceModels.Models;

public partial class Review
{
    public Guid ReviewId { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? UserId { get; set; }

    public bool? IsTop { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
