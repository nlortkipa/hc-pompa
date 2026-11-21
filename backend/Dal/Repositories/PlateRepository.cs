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
    public class PlateRepository : BaseRepository<Plate>, IPlateRepository
    {
        private const string PendingStatus = "Pending";
        private readonly ProjectContext _context;

        public PlateRepository(ProjectContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<IEnumerable<Plate>> GetAllAsync()
        {
            return await _context.Plates
                .Include(x => x.Checkers)
                    .ThenInclude(c => c.User)
                .ToListAsync();
        }

        public override async Task<Plate?> GetByIdAsync(int id)
        {
            return await _context.Plates
                .Include(x => x.Checkers)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PlatePropagationResult> UpdateAndPropagateAsync(Plate plate, string previousNumber, string previousType)
        {
            var result = new PlatePropagationResult();
            await using var transaction = await _context.Database.BeginTransactionAsync();

            await _context.SaveChangesAsync();

            // Reports store the plate number as text, not a foreign key.
            var previousUpper = previousNumber.ToUpper();
            if (!string.Equals(previousNumber, plate.Number, StringComparison.Ordinal))
            {
                result.RenamedReports = await _context.PumpOperations
                    .Where(p => p.PlateNumber.ToUpper() == previousUpper)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.PlateNumber, plate.Number));
            }

            var numberUpper = plate.Number.ToUpper();

            // Accepted/rejected reports keep the type the checker decided on.
            if (!string.Equals(previousType, plate.Type, StringComparison.Ordinal))
            {
                result.RetypedReports = await _context.PumpOperations
                    .Where(p => p.PlateNumber.ToUpper() == numberUpper && p.ReviewStatus == PendingStatus)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.PumpType, plate.Type));
            }

            await SyncPendingReviewRequestsAsync(plate, numberUpper, result);

            await transaction.CommitAsync();
            return result;
        }

        private async Task SyncPendingReviewRequestsAsync(Plate plate, string numberUpper, PlatePropagationResult result)
        {
            var checkers = plate.Checkers
                .Where(c => c.User != null)
                .GroupBy(c => c.UserId)
                .Select(g => g.First().User)
                .ToList();
            if (checkers.Count == 0) return; // plates always have one; never strip a report of all checkers

            var checkerIds = checkers.Select(u => u.Id).ToHashSet();

            var pending = await _context.PumpOperations
                .Where(p => p.PlateNumber.ToUpper() == numberUpper && p.ReviewStatus == PendingStatus)
                .Include(p => p.ReviewRequests)
                .ToListAsync();

            foreach (var operation in pending)
            {
                var requests = operation.ReviewRequests.ToList();

                foreach (var checker in checkers)
                {
                    var alreadyHas = requests.Any(r =>
                        r.CheckerUserId == checker.Id
                        // Request whose user link was lost: matched by the address it was sent to.
                        || (r.CheckerUserId == null
                            && !string.IsNullOrWhiteSpace(r.CheckerGmail)
                            && string.Equals(r.CheckerGmail, checker.Email, StringComparison.OrdinalIgnoreCase)));
                    if (alreadyHas) continue;

                    var request = new PumpOperationReviewRequest
                    {
                        PumpOperationId = operation.Id,
                        CheckerUserId = checker.Id,
                        CheckerGmail = string.IsNullOrWhiteSpace(checker.Email) ? null : checker.Email,
                        Token = Guid.NewGuid().ToString("N"),
                        Status = PendingStatus,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.PumpOperationReviewRequests.Add(request);
                    result.AddedReviewRequests.Add(request);
                }

                // Checkers taken off the plate can no longer decide its pending reports.
                foreach (var removed in requests.Where(r => r.CheckerUserId.HasValue && !checkerIds.Contains(r.CheckerUserId.Value)))
                {
                    _context.PumpOperationReviewRequests.Remove(removed);
                    result.RemovedReviewRequests++;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
