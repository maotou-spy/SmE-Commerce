using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class ProductCategory
{
    public uint ProductCategoryId { get; set; }

    public uint? ProductId { get; set; }

    public uint? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Product? Product { get; set; }
}
