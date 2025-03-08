using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

[Index("OrderCode", Name = "Orders_orderCode_key", IsUnique = true)]
[Index("CreatedAt", Name = "idx_orders_createdat", AllDescending = true)]
[Index("Status", Name = "idx_orders_status")]
[Index("UserId", Name = "idx_orders_userid")]
public class Order
{
    [Key]
    [Column("orderId")]
    public Guid OrderId { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("addressId")]
    public Guid AddressId { get; set; }

    [Column("shippingCode", TypeName = "character varying")]
    public string? ShippingCode { get; set; }

    [Column("totalAmount")]
    [Precision(15, 0)]
    public decimal TotalAmount { get; set; }

    [Column("discountCodeId")]
    public Guid? DiscountCodeId { get; set; }

    [Column("pointsEarned")]
    public int PointsEarned { get; set; }

    /// <summary>
    ///     Values: pending, processing, completed, cancelled, rejected, returned
    /// </summary>
    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("createById")]
    public Guid? CreateById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    public Guid? ModifiedById { get; set; }

    [Column("note", TypeName = "character varying")]
    public string? Note { get; set; }

    [Column("discountamount")]
    [Precision(15, 0)]
    public decimal? DiscountAmount { get; set; }

    [Column("orderCode")]
    [StringLength(50)]
    public string OrderCode { get; set; } = null!;

    [Column("estimatedDeliveryDate", TypeName = "timestamp without time zone")]
    public DateTime? EstimatedDeliveryDate { get; set; }

    [Column("actualDeliveryDate", TypeName = "timestamp without time zone")]
    public DateTime? ActualDeliveryDate { get; set; }

    [Column("shippingFee")]
    [Precision(15, 0)]
    public decimal? ShippingFee { get; set; }

    [Column("subTotal")]
    [Precision(15, 0)]
    public decimal? SubTotal { get; set; }

    [Column("cancelReason")]
    [StringLength(200)]
    public string? CancelReason { get; set; }

    [Column("returnReason")]
    [StringLength(200)]
    public string? ReturnReason { get; set; }

    [Column("pointsUsed")]
    public int PointsUsed { get; set; }

    [ForeignKey("AddressId")]
    [InverseProperty("Orders")]
    public virtual Address Address { get; set; } = null!;

    [ForeignKey("CreateById")]
    [InverseProperty("OrderCreateBies")]
    public virtual User? CreateBy { get; set; }

    [ForeignKey("DiscountCodeId")]
    [InverseProperty("Orders")]
    public virtual DiscountCode? DiscountCode { get; set; }

    [ForeignKey("ModifiedById")]
    [InverseProperty("OrderModifiedBies")]
    public virtual User? ModifiedBy { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Order")]
    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } =
        new List<OrderStatusHistory>();

    [InverseProperty("Order")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey("UserId")]
    [InverseProperty("OrderUsers")]
    public virtual User User { get; set; } = null!;
}
