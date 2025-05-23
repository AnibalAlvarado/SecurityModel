using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dtos;
using Entity.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Business.Implementations
{
    public class CryptoBusiness : ICryptoBusiness
    {
        private readonly ICryptoData _cryptoData;
        private readonly ICryptoApiService _cryptoApiService;
        private readonly IMapper _mapper;

        public CryptoBusiness(ICryptoData cryptoData, ICryptoApiService cryptoApiService, IMapper mapper)
        {
            _cryptoData = cryptoData;
            _cryptoApiService = cryptoApiService;
            _mapper = mapper;
        }

        public async Task ImportFromApiAsync()
        {
            var externalCryptos = await _cryptoApiService.GetTopCryptosAsync(5);

            var cryptos = externalCryptos.Select(c =>
            {
                var entity = _mapper.Map<Crypto>(c);
                entity.RetrievedAt = DateTime.UtcNow;
                return entity;
            });

            await _cryptoData.SaveAsync(cryptos);
        }

        public async Task<List<CryptoDto>> GetAllAsync()
        {
            var entities = await _cryptoData.GetAllAsync();
            return entities.Select(c => _mapper.Map<CryptoDto>(c)).ToList();
        }
    }
}
