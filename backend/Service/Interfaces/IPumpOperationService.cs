using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IPumpOperationService
    {
        Task<PumpOperationDto> CreateAsync(CreatePumpOperationDto dto, int operatorId);
        /// <summary>
        /// Edits the caller's own pending operation; existing review links keep working.
        /// Null when not found / not owned; throws ConflictException when no longer pending.
        /// </summary>
        Task<PumpOperationUpdateResult?> UpdateAsync(int id, CreatePumpOperationDto dto, int operatorId);
        /// <summary>Deletes the caller's own pending operation. False when not found / not owned; throws ConflictException when no longer pending.</summary>
        Task<PumpOperationDeleteResult?> DeleteAsync(int id, int operatorId);
        Task<IEnumerable<PumpOperationDto>> GetAllAsync(int userId, string role, PumpOperationFilterDto? filter = null);
        Task<PumpOperationDto?> GetByIdAsync(int id, int userId, string role);
        Task<IEnumerable<PumpOperationReviewRequestLinkDto>> GetReviewRequestsForOperationAsync(int pumpOperationId);
        Task<IEnumerable<PumpOperationReviewDto>> GetReviewsForCheckerAsync(int checkerUserId);
        Task<PumpOperationReviewDto?> GetReviewByTokenAsync(string token);
        /// <summary>
        /// Records the first decision. With <paramref name="expectedVersion"/>, throws ReviewOutdatedException
        /// if the operation was edited since the checker loaded that version.
        /// </summary>
        Task<PumpOperationReviewDto?> SubmitReviewDecisionAsync(string token, string decision, string? expectedVersion = null);
    }
}
