using Dal.Interfaces;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Repositories
{
    public class CompanyAddressRepository(ProjectContext context) : BaseRepository<CompanyAddress>(context), ICompanyAddressRepository
    {
        private readonly ProjectContext _context = context;

        public async Task DeleteByAddressIdAsync(int addressId)
        {
            var relations = await _context.CompanyAddresses.Where(x => x.AddressId == addressId).ToListAsync();
            if (relations.Any())
            {
                _context.CompanyAddresses.RemoveRange(relations);
                await _context.SaveChangesAsync();
            }
        }
    }
}
