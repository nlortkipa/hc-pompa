using Data.Entities;
using Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dal.Interfaces
{
    public interface ICompanyRepository : IBaseRepository<Company>
    {
        Task<IEnumerable<Address>> GetAddressesAsync(int id);
    }
}
