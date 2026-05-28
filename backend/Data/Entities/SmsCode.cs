using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    [Table("SmsCodes")]
    public class SmsCode
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(4)]
        public string Code { get; set; } = null!;
        [Required]
        public DateTime ExpiresAt { get; set; }

        public User? User { get; set; }
    }
}
