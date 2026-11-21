using Service.DTOs;
using System;

namespace Service.Exceptions
{
    /// <summary>
    /// The checker decided on a version of the operation that has since been edited.
    /// Carries the current data so the client can show it without another request.
    /// </summary>
    public class ReviewOutdatedException(PumpOperationReviewDto current)
        : Exception("The operation was edited after it was loaded")
    {
        public PumpOperationReviewDto Current { get; } = current;
    }
}
