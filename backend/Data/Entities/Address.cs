using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    [Table("Addresses")]
    public class Address
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        [Unicode]
        public string Name { get; set; } = null!;

        public ICollection<CompanyAddress> CompanyAddresses { get; set; } = new List<CompanyAddress>();
        public ICollection<PumpOperation> PumpOperations { get; set; } = new List<PumpOperation>();
    }
}
