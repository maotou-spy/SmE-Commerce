using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmE_CommerceModels.RequestDtos.Category
{
    public class AddCategoryReqDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Category name can only contain letters and spaces.")]
        public required string Name { get; set; } = null!;

        [Required]
        public required string CategoryImage { get; set; }
        
        public string? Description { get; set; }

        public string? Status { get; set; }
    }
}
