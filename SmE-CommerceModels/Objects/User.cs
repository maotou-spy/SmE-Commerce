using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class User
{
    public uint UserId { get; set; }

    public string Role { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? Email { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public uint Point { get; set; }

    /// <summary>
    /// Values: active, inactive, suspended
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? LastLogin { get; set; }

    public DateTime? CreatedDate { get; set; }

    public uint? CreatedById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public uint? ModifiedById { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
