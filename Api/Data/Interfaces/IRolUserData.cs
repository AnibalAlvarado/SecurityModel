using Entity.Dtos;
using Entity.DTOs;
using Entity.Model;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IRolUserData : IRepositoryData<RolUser>
    {
        public Task<IEnumerable<RolUser>> GetAllJoinAsync();

        Task<bool> ExistsAsync(int userId, int roleId);
    }
}
