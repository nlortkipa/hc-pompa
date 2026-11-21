using Data.Entities;
using Data.Infrastructure;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Dal.Interfaces
{
    public interface IPumpOperationReviewRequestRepository : IBaseRepository<PumpOperationReviewRequest>
    {
        Task<PumpOperationReviewRequest?> GetByTokenAsync(string token);
        Task<IEnumerable<PumpOperationReviewRequest>> GetByPumpOperationIdAsync(int pumpOperationId);
        Task<IEnumerable<PumpOperationReviewRequest>> GetByCheckerUserIdAsync(int checkerUserId);
        /// <summary>Points the checker's requests on still-pending reports at their new address (null = no email).</summary>
        Task<int> UpdatePendingCheckerEmailAsync(int checkerUserId, string? email);
        /// <summary>
        /// Sets the decision if the operation is still pending and the token still belongs to it.
        /// When <paramref name="expected"/> is given, it also requires the editable fields to still
        /// equal that snapshot, so a decision never applies to data the checker did not see.
        /// </summary>
        Task<bool> TrySetFirstDecisionAsync(int pumpOperationId, string token, string decision, string reviewerLabel, DateTime reviewedAt, PumpOperation? expected = null);
    }
}
