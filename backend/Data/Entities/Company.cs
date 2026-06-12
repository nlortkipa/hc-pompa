using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    [Table("Companies")]
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public ICollection<CompanyAddress> CompanyAddresses { get; set; } = new List<CompanyAddress>();
        public ICollection<PumpOperation> PumpOperations { get; set; } = new List<PumpOperation>();
    }
}
