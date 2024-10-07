namespace SmE_CommerceModels.Models;

public partial class Setting : Common
{
    public Guid SettingId { get; set; }

    /// <summary>
    /// Values: shopName, address, phone, email, maximumTopReview, privacyPolicy, termsOfService, pointsConversionRate
    /// </summary>
    public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string? Description { get; set; }
}
