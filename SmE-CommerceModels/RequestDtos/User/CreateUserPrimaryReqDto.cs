using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmE_CommerceModels.RequestDtos.User
{
    public class CreateUserPrimaryReqDto
    {
        public Guid RoleId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public Guid CreateById { get; set; }
    }
}
