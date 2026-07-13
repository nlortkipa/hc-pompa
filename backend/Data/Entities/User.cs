using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(25)]
        [Unicode(true)]
        public string FirstName { get; set; } = null!;
        [Required]
        [MaxLength(50)]
        [Unicode(true)]
        public string LastName { get; set; } = null!;
        [Required]
        [MaxLength(11)]
        public string PersonalId { get; set; } = null!;
        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "User";
        [MaxLength(75)]
        [Unicode(true)]
        public string? Username { get; set; }
        public string? AvatarUrl { get; set; }
        [Required]
        public int TokenVersion { get; set; }

        public ICollection<SmsCode> SmsCodes { get; set; } = new List<SmsCode>();
        public ICollection<PumpOperation> PumpOperations { get; set; } = new List<PumpOperation>();
    }
}
