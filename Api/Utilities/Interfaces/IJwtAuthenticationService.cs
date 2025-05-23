using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Interfaces
{
    public interface IJwtAuthenticationService
    {
        string GenerarToken(User usuario, List<string> roles);
    }
}
