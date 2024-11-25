namespace SmE_CommerceModels.Models;

public class ContentProduct
{
    public Guid ContentProductId { get; set; }

    public Guid ContentId { get; set; }

    public Guid ProductId { get; set; }

    public virtual Content Content { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}