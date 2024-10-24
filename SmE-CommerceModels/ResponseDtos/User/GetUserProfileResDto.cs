using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmE_CommerceModels.ResponseDtos.User
{
    public class GetUserProfileResDto
    {
        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public int? Point { get; set; }
    }
}
