using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    [Table("CompanyAddresses")]
    public class CompanyAddress
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int AddressId { get; set; }

        public Company? Company { get; set; }
        public Address? Address { get; set; }
    }
}
