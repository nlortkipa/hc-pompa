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
    public class PumpOperationRepository(ProjectContext context) : BaseRepository<PumpOperation>(context), IPumpOperationRepository
    {
        private const string PendingStatus = "Pending";
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

        public async Task<(PumpOperation Operation, List<PumpOperationReviewRequest> AddedRequests)?> TryUpdatePendingAsync(
            int id,
            int operatorId,
            PumpOperation values,
            IReadOnlyList<PumpOperationReviewRequest> desiredReviewRequests)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            // The status condition makes this race-safe against a checker's accept/reject,
            // which flips the status with the same kind of conditional update.
            var updated = await _context.PumpOperations
                .Where(p => p.Id == id && p.OperatorId == operatorId && p.ReviewStatus == PendingStatus)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.Date, values.Date)
                    .SetProperty(p => p.CompanyId, values.CompanyId)
                    .SetProperty(p => p.AddressId, values.AddressId)
                    .SetProperty(p => p.ConcreteVolume, values.ConcreteVolume)
                    .SetProperty(p => p.AdditionalStanding, values.AdditionalStanding)
                    .SetProperty(p => p.IsIdle, values.IsIdle)
                    .SetProperty(p => p.IsDaily, values.IsDaily)
                    .SetProperty(p => p.DismantleAssemble, values.DismantleAssemble)
                    .SetProperty(p => p.PlateNumber, values.PlateNumber)
                    .SetProperty(p => p.PumpType, values.PumpType)
                    .SetProperty(p => p.DistanceRange, values.DistanceRange)
                    .SetProperty(p => p.Note, values.Note));

            if (updated == 0)
            {
                await transaction.RollbackAsync();
                return null;
            }

            // Keep each remaining checker's request (and so their link); the plate may have
            // changed, so drop checkers no longer assigned and add new ones.
            var existing = await _context.PumpOperationReviewRequests
                .Where(r => r.PumpOperationId == id)
                .ToListAsync();

            var added = new List<PumpOperationReviewRequest>();
            var kept = new HashSet<PumpOperationReviewRequest>();
            foreach (var desired in desiredReviewRequests)
            {
                var match = existing.FirstOrDefault(r => !kept.Contains(r) && IsSameChecker(r, desired));
                if (match != null)
                {
                    kept.Add(match);
                    continue;
                }

                desired.PumpOperationId = id;
                _context.PumpOperationReviewRequests.Add(desired);
                added.Add(desired);
            }

            _context.PumpOperationReviewRequests.RemoveRange(existing.Where(r => !kept.Contains(r)));
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // AsNoTracking so a previously tracked (now stale) instance is not returned.
            var operation = await _context.PumpOperations
                .AsNoTracking()
                .Include(p => p.Operator)
                .Include(p => p.Company)
                .Include(p => p.Address)
                .FirstAsync(p => p.Id == id);

            return (operation, added);
        }

        private static bool IsSameChecker(PumpOperationReviewRequest existing, PumpOperationReviewRequest desired)
        {
            if (existing.CheckerUserId.HasValue)
            {
                return existing.CheckerUserId == desired.CheckerUserId;
            }

            // Request whose user was deleted / predates user-based checkers: match on the mailed address.
            return !string.IsNullOrWhiteSpace(existing.CheckerGmail)
                && string.Equals(existing.CheckerGmail, desired.CheckerGmail, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> TryDeletePendingAsync(int id, int operatorId)
        {
            // Review requests are removed by the FK's ON DELETE CASCADE.
            var deleted = await _context.PumpOperations
                .Where(p => p.Id == id && p.OperatorId == operatorId && p.ReviewStatus == PendingStatus)
                .ExecuteDeleteAsync();

            return deleted > 0;
        }
    }
}
