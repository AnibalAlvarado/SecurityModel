using Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ICryptoBusiness
    {
        Task ImportFromApiAsync();
        Task<List<CryptoDto>> GetAllAsync();
    }
}
