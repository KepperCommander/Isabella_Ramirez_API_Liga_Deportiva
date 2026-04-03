using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories;


public interface ISponsorRepository : IGenericRepository<Sponsor>

{
    Task<Sponsor?> GetByNameAsync(string name);
}