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
        public User Operator { get; set; } = null!;
        [Required]
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
        [Required]
        public int AddressId { get; set; }
        public Address Address { get; set; } = null!;
        [Required]
        public int PumpPlateId { get; set; }
        [Required]
        public decimal ConcreteVolume { get; set; }
        [Required]
        public int PumpRelocation { get; set; }
        [Required]
        public int StationaryRelocation { get; set; }
        [Required]
        public int HydraulicRelocation { get; set; }
        [Required]
        public int StationaryReposition { get; set; }
        [Required]
        public decimal IdleTime { get; set; }
        [Required]
        public int DismantleAssemble { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
