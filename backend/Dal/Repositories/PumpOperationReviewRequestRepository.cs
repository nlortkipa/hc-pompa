using Dal.Interfaces;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
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
                .AsNoTracking()
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
                .AsNoTracking()
                .Where(r => r.PumpOperationId == pumpOperationId)
                .ToListAsync();
        }

        public async Task<bool> TrySetFirstDecisionAsync(
            int pumpOperationId,
            string decision,
            string checkerEmail,
            DateTime reviewedAt)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var updatedOperations = await _context.PumpOperations
                .Where(operation => operation.Id == pumpOperationId && operation.ReviewStatus == "Pending")
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(operation => operation.ReviewStatus, decision)
                    .SetProperty(operation => operation.ReviewedAt, reviewedAt)
                    .SetProperty(operation => operation.ReviewedByCheckerGmail, checkerEmail));

            if (updatedOperations == 0)
            {
                await transaction.RollbackAsync();
                return false;
            }

            await _context.PumpOperationReviewRequests
                .Where(request => request.PumpOperationId == pumpOperationId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(request => request.Status, decision)
                    .SetProperty(request => request.ReviewedAt, reviewedAt));

            await transaction.CommitAsync();
            return true;
        }
    }
}
