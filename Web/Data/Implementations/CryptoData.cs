using Data.Interfaces;
using Entity.Context;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Implementations
{
    public class CryptoData : ICryptoData
    {
        private readonly ApplicationDbContext _context;

        public CryptoData(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(IEnumerable<Crypto> cryptos)
        {
            await _context.Cryptos.AddRangeAsync(cryptos);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Crypto>> GetAllAsync()
        {
            return await _context.Cryptos.OrderByDescending(c => c.RetrievedAt).ToListAsync();
        }
    }
}
