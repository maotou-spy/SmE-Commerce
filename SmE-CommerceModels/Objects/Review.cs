using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class Review
{
    public uint ReviewId { get; set; }

    public uint? ProductId { get; set; }

    public ulong? IsFeature { get; set; }

    public uint? Rating { get; set; }

    public string? Comment { get; set; }

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public uint? CreatedById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public uint? ModifiedById { get; set; }

    public virtual Product? Product { get; set; }
}
