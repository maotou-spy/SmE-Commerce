using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class ContentCategoryMap
{
    public uint ContentCategoryMapId { get; set; }

    public uint? ContentId { get; set; }

    public uint? BlogCategoryId { get; set; }

    public virtual BlogCategory? BlogCategory { get; set; }

    public virtual Content? Content { get; set; }
}
