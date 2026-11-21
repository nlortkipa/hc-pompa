using System.Collections.Generic;

namespace Service.DTOs
{
    public class PlateDTO
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool IsStationary { get; set; }
        /// <summary>Users who review reports for this plate. Sent by the client on create/update.</summary>
        public List<int> CheckerUserIds { get; set; } = new();
        /// <summary>Read-only details of the checkers, filled on reads.</summary>
        public List<PlateCheckerDTO> Checkers { get; set; } = new();
    }

    public class PlateCheckerDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Email { get; set; }
    }

    public class PlateUpdateResultDTO
    {
        /// <summary>Reports (any status) whose plate number was renamed.</summary>
        public int RenamedReports { get; set; }
        /// <summary>Pending reports whose pump type was updated.</summary>
        public int RetypedReports { get; set; }
        /// <summary>Pending review requests removed because their checker left the plate.</summary>
        public int RemovedReviewRequests { get; set; }
        /// <summary>Pending review requests created for newly assigned checkers.</summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public List<PumpOperationReviewRequestLinkDto> AddedReviewRequests { get; set; } = new();
        public int AddedReviewRequestCount => AddedReviewRequests.Count;
    }
}
