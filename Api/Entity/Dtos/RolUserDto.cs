using Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class RolUserDto : BaseDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; } = null!;
        public int RolId { get; set; }
        public string? RolName { get; set; } = null!;
    }


}
