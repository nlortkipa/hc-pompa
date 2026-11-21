using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    [Table("PumpOperations")]
    public class PumpOperation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int OperatorId { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int AddressId { get; set; }
        [Required]
        public decimal ConcreteVolume { get; set; }
        [Required]
        public int AdditionalStanding { get; set; }
        [Required]
        public bool IsIdle { get; set; }
        [Required]
        public int DismantleAssemble { get; set; }
        [Required]
        public string PlateNumber { get; set; } = null!;
        [Required]
        public string PumpType { get; set; } = string.Empty;
        public string? DistanceRange { get; set; }
        public string? Note { get; set; }
        [Required]
        public string ReviewStatus { get; set; } = "Pending";
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedByCheckerGmail { get; set; }

        public User Operator { get; set; } = null!;
        public Company Company { get; set; } = null!;
        public Address Address { get; set; } = null!;
        public ICollection<PumpOperationReviewRequest> ReviewRequests { get; set; } = new List<PumpOperationReviewRequest>();
    }
}
