using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmE_CommerceModels.RequestDtos.User
{
    public class UpdateUserProfileReqDto
    {
        public string? FullName { get; set; }

        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number invalid")]
        public string? Phone { get; set; }
    }
}
