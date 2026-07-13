using Dal.Interfaces;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dal.Repositories
{
    public class PumpOperationReviewRequestRepository(ProjectContext context) : BaseRepository<PumpOperationReviewRequest>(context), IPumpOperationReviewRequestRepository
    {
        private readonly ProjectContext _context = context;

        public async Task<PumpOperationReviewRequest?> GetByTokenAsync(string token)
        {
            return await _context.PumpOperationReviewRequests
                .Include(r => r.PumpOperation)
                    .ThenInclude(p => p.Operator)
                .Include(r => r.PumpOperation)
                    .ThenInclude(p => p.Company)
                .Include(r => r.PumpOperation)
                    .ThenInclude(p => p.Address)
                .FirstOrDefaultAsync(r => r.Token == token);
        }

        public async Task<IEnumerable<PumpOperationReviewRequest>> GetByPumpOperationIdAsync(int pumpOperationId)
        {
            return await _context.PumpOperationReviewRequests
                .Where(r => r.PumpOperationId == pumpOperationId)
                .ToListAsync();
        }
    }
}
