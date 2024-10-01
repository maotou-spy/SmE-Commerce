using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class Category
{
    public uint CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? CategoryImage { get; set; }

    public string? CategoryImageHash { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public uint? CreatedById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public uint? ModifiedById { get; set; }

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}
