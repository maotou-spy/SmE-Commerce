using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class Setting
{
    public uint SettingId { get; set; }

    /// <summary>
    /// Values: shopName, address, phone, email, logoUrl, maximumTopReview, privacyPolicy, termsOfService, pointsConversionRate
    /// </summary>
    public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public uint? CreatedById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public uint? ModifiedById { get; set; }
}
