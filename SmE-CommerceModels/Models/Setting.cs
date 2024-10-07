using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Models;

public partial class Setting
{
    public Guid SettingId { get; set; }

    /// <summary>
    /// Values: shopName, address, phone, email, maximumTopReview, privacyPolicy, termsOfService, pointsConversionRate
    /// </summary>
    public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? CreateById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public Guid? ModifiedById { get; set; }
}
