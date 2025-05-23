using Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Interfaces
{
    public interface ICryptoApiService
    {
        Task<List<ExternalCryptoDto>> GetTopCryptosAsync(int count);
    }
}
