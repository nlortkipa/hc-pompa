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
        [MaxLength(15)]
        public string Number { get; set; } = null!;
        [Required]
        public DateTime CreatedAt { get; set; }

        public ICollection<SmsCode> SmsCodes { get; set; } = new List<SmsCode>();
    }
}
