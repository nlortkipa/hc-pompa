using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    [Table("PumpOperationReviewRequests")]
    public class PumpOperationReviewRequest
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PumpOperationId { get; set; }
        [Required]
        public string CheckerGmail { get; set; } = null!;
        [Required]
        public string Token { get; set; } = null!;
        [Required]
        public string Status { get; set; } = "Pending";
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }

        public PumpOperation PumpOperation { get; set; } = null!;
    }
}
