using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmE_CommerceModels.Models;

[Table("ContentCategoryMap")]
public partial class ContentCategoryMap
{
    [Key]
    [Column("contentCategoryMapId")]
    public Guid ContentCategoryMapId { get; set; }

    [Column("contentId")]
    public Guid? ContentId { get; set; }

    [Column("blogCategoryId")]
    public Guid? BlogCategoryId { get; set; }

    [ForeignKey("BlogCategoryId")]
    [InverseProperty("ContentCategoryMaps")]
    public virtual BlogCategory? BlogCategory { get; set; }

    [ForeignKey("ContentId")]
    [InverseProperty("ContentCategoryMaps")]
    public virtual Content? Content { get; set; }
}
