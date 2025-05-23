using AutoMapper;
using Data.Interfaces;
using Entity.Contexts;
using Entity.DTOs;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations
{
    public class RolFormPermissionData : RepositoryData<RolFormPermission>, IRolFormPermissionData
    {
        public RolFormPermissionData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService)
            : base(context, configuration,auditService, currentUserService)
        {

        }

        public async Task<IEnumerable<RolFormPermission>> GetAllJoinAsync()
        {
            await AuditAsync("GetAllJoinAsync");

            return await _context.RolFormPermission
                .Include(x => x.Rol)
                .Include(x => x.Form)
                .Include(x => x.Permission)
                .ToListAsync();
        }
    }
}
