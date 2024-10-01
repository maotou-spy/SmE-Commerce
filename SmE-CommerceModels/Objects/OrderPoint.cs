using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class OrderPoint
{
    public uint OrderPointId { get; set; }

    public uint? OrderId { get; set; }

    public uint? PointsEarned { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Order? Order { get; set; }
}
