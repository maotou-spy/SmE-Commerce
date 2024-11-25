namespace SmE_CommerceModels.Models;

public class ContentCategoryMap
{
    public Guid ContentCategoryMapId { get; set; }

    public Guid? ContentId { get; set; }

    public Guid? BlogCategoryId { get; set; }

    public virtual BlogCategory? BlogCategory { get; set; }

    public virtual Content? Content { get; set; }
}