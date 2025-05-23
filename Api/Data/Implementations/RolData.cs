using AutoMapper;
using Data.Interfaces;
using Entity.Contexts;
using Entity.DTOs;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations
{
    public class RolData : RepositoryData<Rol>, IRolData
    {
        public RolData(ApplicationDbContext context, IConfiguration configuration,IAuditService auditService, ICurrentUserService currentUserService)
            : base(context, configuration,  auditService, currentUserService)
        {

        }

        public async Task<Rol> GetByNameAsync(string name)
        {

            try
            {
                return await _context.Set<Rol>()
                        .FirstOrDefaultAsync(r => r.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            }
            catch (Exception ex)
            {
                throw new DataException("Error al obtener el rol por nombre", ex);
            }
        }
    }
}
