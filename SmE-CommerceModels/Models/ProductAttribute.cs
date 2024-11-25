namespace SmE_CommerceModels.Models;

public class ProductAttribute
{
    public Guid Attributeid { get; set; }

    public Guid Productid { get; set; }

    public string Attributename { get; set; } = null!;

    public string Attributevalue { get; set; }

    public virtual Product Product { get; set; } = null!;
}