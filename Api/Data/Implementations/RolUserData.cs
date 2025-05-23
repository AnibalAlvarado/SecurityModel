using AutoMapper;
using Data.Interfaces;
using Entity.Contexts;
using Entity.DTOs;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Exceptions;
using Utilities.Interfaces;

namespace Data.Implementations
{
    public class RolUserData : RepositoryData<RolUser>, IRolUserData
    {
        private readonly ILogger<RolUserData> _logger;

        public RolUserData(ApplicationDbContext context, IConfiguration configuration,  ILogger<RolUserData> logger, IAuditService auditService, ICurrentUserService currentUserService)
            : base(context, configuration, auditService, currentUserService)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<RolUser>> GetAllJoinAsync()
        {
            await AuditAsync("GetAllJoinAsync");

            return await _context.RolUser
                .Include(x => x.User)
                .Include(x => x.Rol)
                .ToListAsync();
        }


        public override async Task<IEnumerable<RolUser>> GetAll()
        {
            var entities = await _context.RolUser
                .Include(ru => ru.User)
                .Include(ru => ru.Rol)
                .ToListAsync();

            await AuditAsync("GetAll");

            return entities;
        }
        public override async Task<RolUser> GetById(int id)
        {
            await AuditAsync("GetById", id);
            return await _context.RolUser
                .Include(ru => ru.User)
                .Include(ru => ru.Rol)
                .FirstOrDefaultAsync(ru => ru.Id == id);
        }

        public async Task<bool> ExistsAsync(int userId, int roleId)
        {
            try
            {
                return await _context.Set<RolUser>()
                        .AnyAsync(ur => ur.UserId == userId && ur.RolId == roleId);
            }
            catch (Exception ex)
            {

                throw  new DataException("Error al obtener el rol por nombre", ex);
            }
        }
    }
}
