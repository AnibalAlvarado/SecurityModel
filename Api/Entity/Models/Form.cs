
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Form : GenericModel
    {

        public string Description { get; set; }

        public virtual ICollection<FormModule> FormModules { get; set; } = new List<FormModule>();
        public virtual ICollection<RolFormPermission> RolFormPermission { get; set; } = new List<RolFormPermission>();

    }
}
