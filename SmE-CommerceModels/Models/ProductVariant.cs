namespace SmE_CommerceModels.Models;

public partial class ProductVariant : Common
{
    public Guid VariantId { get; set; }

    public Guid ProductId { get; set; }

    public string VariantName { get; set; } = null!;

    public string? Sku { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int SoldQuantity { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<VariantAttribute> VariantAttributes { get; set; } = new List<VariantAttribute>();
}
