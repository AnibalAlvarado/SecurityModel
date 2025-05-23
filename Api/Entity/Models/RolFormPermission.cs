using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class RolFormPermission : BaseModel
    {
        public int RolId { get; set; }
        public int FormId { get; set; }
        public int PermissionId { get; set; }

        public Rol Rol { get; set; }
        public Form Form { get; set; }
        public Permission Permission { get; set; }

    }
}
