using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Service.DTOs
{
    public class SmsCodeDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserNumber { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }

    public class VerifySmsCodeDTO
    {
        public int UserId { get; set; }
        public string Code { get; set; } = null!;
    }
}
