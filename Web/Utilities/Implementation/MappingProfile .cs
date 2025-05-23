using AutoMapper;
using Entity.Dtos;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Implementation
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<Crypto, CryptoDto>().ReverseMap();

            // DTO externo → Entity
            CreateMap<ExternalCryptoDto, Crypto>()
                .ForMember(dest => dest.CurrentPrice, opt => opt.MapFrom(src => src.CurrentPrice))
                .ForMember(dest => dest.RetrievedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
