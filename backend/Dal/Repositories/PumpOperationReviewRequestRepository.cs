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
                .Include(r => r.CheckerUser)
                .FirstOrDefaultAsync(r => r.Token == token);
        }

        public async Task<IEnumerable<PumpOperationReviewRequest>> GetByCheckerUserIdAsync(int checkerUserId)
        {
            return await _context.PumpOperationReviewRequests
                .AsNoTracking()
                .Include(r => r.PumpOperation)
                    .ThenInclude(p => p.Operator)
                .Include(r => r.PumpOperation)
                    .ThenInclude(p => p.Company)
                .Include(r => r.PumpOperation)
                    .ThenInclude(p => p.Address)
                .Include(r => r.CheckerUser)
                .Where(r => r.CheckerUserId == checkerUserId)
                .ToListAsync();
        }

        public async Task<int> UpdatePendingCheckerEmailAsync(int checkerUserId, string? email)
        {
            return await _context.PumpOperationReviewRequests
                .Where(r => r.CheckerUserId == checkerUserId && r.PumpOperation.ReviewStatus == "Pending")
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.CheckerGmail, email));
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
            string token,
            string decision,
            string reviewerLabel,
            DateTime reviewedAt,
            PumpOperation? expected = null)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var query = _context.PumpOperations
                // The token check rejects links of checkers removed from the operation by an edit.
                .Where(operation => operation.Id == pumpOperationId
                    && operation.ReviewStatus == "Pending"
                    && operation.ReviewRequests.Any(request => request.Token == token));

            if (expected != null)
            {
                query = query.Where(p =>
                    p.Date == expected.Date
                    && p.CompanyId == expected.CompanyId
                    && p.AddressId == expected.AddressId
                    && p.ConcreteVolume == expected.ConcreteVolume
                    && p.AdditionalStanding == expected.AdditionalStanding
                    && p.IsIdle == expected.IsIdle
                    && p.IsDaily == expected.IsDaily
                    && p.DismantleAssemble == expected.DismantleAssemble
                    && p.PlateNumber == expected.PlateNumber
                    && p.PumpType == expected.PumpType
                    && p.DistanceRange == expected.DistanceRange
                    && p.Note == expected.Note);
            }

            var updatedOperations = await query
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(operation => operation.ReviewStatus, decision)
                    .SetProperty(operation => operation.ReviewedAt, reviewedAt)
                    .SetProperty(operation => operation.ReviewedByCheckerGmail, reviewerLabel));

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
