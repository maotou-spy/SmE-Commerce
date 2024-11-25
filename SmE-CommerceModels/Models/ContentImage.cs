namespace SmE_CommerceModels.Models;

public class ContentImage
{
    public Guid ContentImageId { get; set; }

    public Guid? ContentId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? ImageHash { get; set; }

    public string? AltText { get; set; }

    public virtual Content? Content { get; set; }
}