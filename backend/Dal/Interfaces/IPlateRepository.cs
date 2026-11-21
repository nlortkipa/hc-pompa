using Data.Entities;
using Data.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dal.Interfaces
{
    public interface IPlateRepository : IBaseRepository<Plate>
    {
        /// <summary>
        /// Saves the (tracked, already modified) plate and, in the same transaction, carries the
        /// change over to reports that copy plate data:
        /// number → every report on the plate; type → pending reports;
        /// checkers → review requests of pending reports (added and removed).
        /// </summary>
        Task<PlatePropagationResult> UpdateAndPropagateAsync(Plate plate, string previousNumber, string previousType);
    }

    public class PlatePropagationResult
    {
        public int RenamedReports { get; set; }
        public int RetypedReports { get; set; }
        public int RemovedReviewRequests { get; set; }
        /// <summary>Requests created for newly assigned checkers on pending reports (need an email).</summary>
        public List<PumpOperationReviewRequest> AddedReviewRequests { get; set; } = new();
    }
}
