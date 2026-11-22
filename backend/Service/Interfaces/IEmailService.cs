using Service.DTOs;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public enum ReviewEmailKind
    {
        /// <summary>A checker is asked to review an operation for the first time.</summary>
        Created,
        /// <summary>The operator edited an operation the checker was already asked to review.</summary>
        Updated,
        /// <summary>The operator deleted an operation the checker was asked to review.</summary>
        Deleted
    }

    public interface IEmailService
    {
        Task SendPumpOperationReviewEmailAsync(ReviewEmailKind kind, PumpOperationDto operation, string checkerEmail, string reviewUrl);
    }
}
