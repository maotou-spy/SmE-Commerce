namespace SmE_CommerceModels.Models;

public partial class Product : Common
{
    public Guid ProductId { get; set; }

    public string? ProductCode { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int SoldQuantity { get; set; }

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public string? Slug { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public List<string>? Keywords { get; set; }

    public bool IsTopSeller { get; set; } = false;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<ContentProduct> ContentProducts { get; set; } = new List<ContentProduct>();

    public virtual ICollection<DiscountProduct> DiscountProducts { get; set; } = new List<DiscountProduct>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductAttribute> ProductAttributes { get; set; } = new List<ProductAttribute>();

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
