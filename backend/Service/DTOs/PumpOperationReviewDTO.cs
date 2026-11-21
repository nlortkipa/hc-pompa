using System;

namespace Service.DTOs
{
    public class PumpOperationReviewRequestLinkDto
    {
        public int Id { get; set; }
        public int PumpOperationId { get; set; }
        public string Token { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string CheckerGmail { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }

    public class PumpOperationReviewDto
    {
        public string Token { get; set; } = null!;
        public string ReviewStatus { get; set; } = null!;
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedByCheckerGmail { get; set; }
        public string CheckerGmail { get; set; } = null!;
        public int PumpOperationId { get; set; }
        public DateTime Date { get; set; }
        public int OperatorId { get; set; }
        public string OperatorFirstName { get; set; } = null!;
        public string OperatorLastName { get; set; } = null!;
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public int AddressId { get; set; }
        public string AddressName { get; set; } = null!;
        public decimal ConcreteVolume { get; set; }
        public int AdditionalStanding { get; set; }
        public bool IsIdle { get; set; }
        public int DismantleAssemble { get; set; }
        public string PlateNumber { get; set; } = null!;
        public string PumpType { get; set; } = null!;
        public string? DistanceRange { get; set; }
        public string? Note { get; set; }
    }
}
