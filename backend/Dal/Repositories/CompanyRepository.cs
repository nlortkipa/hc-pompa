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

        public override async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _context.Companies
                .Where(company => company.IsActive)
                .OrderBy(company => company.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Address>> GetAddressesAsync(int companyId)
        {
            return await _context.CompanyAddresses
                .Where(ca => ca.CompanyId == companyId && ca.Address!.IsActive)
                .Include(ca => ca.Address)
                .Select(ca => ca.Address!)
                .ToListAsync();
        }
    }
}
