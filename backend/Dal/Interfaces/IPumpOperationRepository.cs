using Data.Entities;
using Data.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dal.Interfaces
{
    public interface IPumpOperationRepository : IBaseRepository<PumpOperation>
    {
        /// <summary>
        /// Atomically overwrites the editable fields of a still-pending operation owned by
        /// <paramref name="operatorId"/> and syncs its review requests with <paramref name="desiredReviewRequests"/>:
        /// checkers that remain keep their existing request (same link), removed checkers lose theirs,
        /// and new checkers get the supplied request. Returns null when the operation is no longer
        /// pending (or not owned); otherwise the reloaded operation and the requests that were added.
        /// </summary>
        Task<(PumpOperation Operation, List<PumpOperationReviewRequest> AddedRequests)?> TryUpdatePendingAsync(
            int id,
            int operatorId,
            PumpOperation values,
            IReadOnlyList<PumpOperationReviewRequest> desiredReviewRequests);

        /// <summary>Deletes the operation only if it is still pending and owned by <paramref name="operatorId"/>.</summary>
        Task<bool> TryDeletePendingAsync(int id, int operatorId);
    }
}
