using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class ContentImage
{
    public uint ContentImageId { get; set; }

    public uint? ContentId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? ImageHash { get; set; }

    public string? AltText { get; set; }

    public virtual Content? Content { get; set; }
}
