using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class ProductImage
{
    public uint ImageId { get; set; }

    public uint? ProductId { get; set; }

    public string Url { get; set; } = null!;

    public string? ImageHash { get; set; }

    public string? AltText { get; set; }

    public virtual Product? Product { get; set; }
}
