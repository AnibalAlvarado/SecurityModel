using AutoMapper;
using Data.Interfaces;
using Entity.Contexts;
using Entity.DTOs;
using Entity.Model;
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
    public class PersonData : RepositoryData<Person>, IPersonData
    {
        public PersonData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService)
            : base(context, configuration, auditService, currentUserService)
        {

        }
    }
}
