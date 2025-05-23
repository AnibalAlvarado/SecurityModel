using Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class UserDto : BaseDto
    {
        public string UserName { get; set; } 
        public string Email { get; set; }
        public string Password { get; set; }
        public int PersonId { get; set; }
        public string? PersonName { get; set; } = null!;
    }
}
