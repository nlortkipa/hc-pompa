using Data.Entities;
using Data.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dal.Interfaces
{
    public interface IPumpOperationReviewRequestRepository : IBaseRepository<PumpOperationReviewRequest>
    {
        Task<PumpOperationReviewRequest?> GetByTokenAsync(string token);
        Task<IEnumerable<PumpOperationReviewRequest>> GetByPumpOperationIdAsync(int pumpOperationId);
    }
}
