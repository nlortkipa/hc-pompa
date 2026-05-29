using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Service.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUserDTO
    {
        public string Number { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateUserDTO
    {
        public string Number { get; set; } = null!;
    }
}
