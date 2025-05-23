using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.DTOs;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementations
{
    public class PersonBusiness : RepositoryBusiness<Person, PersonDto>, IPersonBusiness
    {
        private readonly IPersonData _data;
        public PersonBusiness(IPersonData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
        }
    }
}
