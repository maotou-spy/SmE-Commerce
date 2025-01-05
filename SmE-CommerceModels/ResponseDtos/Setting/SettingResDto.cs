namespace SmE_CommerceModels.ResponseDtos.setting;

public class SettingResDto
{
    public Guid SettingId { get; set; }

    public required string Key { get; set; }

    public required string Value { get; set; }

    public string? Description { get; set; }
}
