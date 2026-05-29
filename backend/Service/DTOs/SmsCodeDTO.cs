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
        public DateTime ExpiresAt { get; set; }
    }

    public class CreateSmsCodeDTO
    {
        public int UserId { get; set; }
        public string Code { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }

    public class UpdateSmsCodeDTO
    {
        public int UserId { get; set; }
        public string Code { get; set; } = null!;
    }
}
