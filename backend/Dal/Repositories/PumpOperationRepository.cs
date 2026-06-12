using Dal.Interfaces;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Repositories
{
    public class PumpOperationRepository(ProjectContext context) : BaseRepository<PumpOperation>(context), IPumpOperationRepository
    {
        private readonly ProjectContext _context = context;

        public override async Task<IEnumerable<PumpOperation>> GetAllAsync()
        {
            return await _context.PumpOperations
                .Include(p => p.Operator)
                .Include(p => p.Company)
                .Include(p => p.Address)
                .ToListAsync();
        }

        public override async Task<PumpOperation?> GetByIdAsync(int id)
        {
            return await _context.PumpOperations
                .Include(p => p.Operator)
                .Include(p => p.Company)
                .Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
