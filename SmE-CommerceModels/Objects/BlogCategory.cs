using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class BlogCategory
{
    public uint BlogCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public uint? CreatedById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public uint? ModifiedById { get; set; }

    public virtual ICollection<ContentCategoryMap> ContentCategoryMaps { get; set; } = new List<ContentCategoryMap>();
}
