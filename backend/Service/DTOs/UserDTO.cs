using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Service.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PersonalId { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string Role { get; set; } = null!;
        public string? Username { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Email { get; set; }
        [JsonIgnore]
        public int TokenVersion { get; set; }
    }

    public class CreateUserDTO
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PersonalId { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string Role { get; set; } = "User";
        public string? Username { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Email { get; set; }
    }

    public class UpdateUserDTO
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PersonalId { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? Email { get; set; }
    }

    public class UpdateProfileDTO
    {
        public string Username { get; set; } = null!;
        public string? AvatarUrl { get; set; }
    }
}
