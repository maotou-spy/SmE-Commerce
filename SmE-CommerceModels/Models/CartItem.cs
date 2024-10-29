namespace SmE_CommerceModels.Models;

public class CartItem
{
    public Guid CartItemId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
