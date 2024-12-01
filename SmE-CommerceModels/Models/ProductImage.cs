namespace SmE_CommerceModels.Models;

public class ProductImage
{
    public Guid ImageId { get; set; }

    public Guid? ProductId { get; set; }

    public string Url { get; set; } = null!;

    public string? AltText { get; set; }

    public virtual Product? Product { get; set; }
}
