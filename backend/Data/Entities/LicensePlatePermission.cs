using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    [Table("LicensePlatePermissions")]
    public class LicensePlatePermission
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string LicensePlate { get; set; } = null!;
        [Required]
        public int Permission { get; set; }
    }
}
