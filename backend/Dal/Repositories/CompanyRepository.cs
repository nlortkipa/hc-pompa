using Dal.Interfaces;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dal.Repositories
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        private readonly ProjectContext _context;

        public CompanyRepository(ProjectContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Address>> GetAddressesAsync(int companyId)
        {
            return await _context.CompanyAddresses
                .Where(ca => ca.CompanyId == companyId)
                .Include(ca => ca.Address)
                .Select(ca => ca.Address!)
                .ToListAsync();
        }
    }
}
