using System.Collections.Generic;

namespace Service.DTOs
{
    public class PlateDTO
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool IsStationary { get; set; }
        /// <summary>Users who review reports for this plate. Sent by the client on create/update.</summary>
        public List<int> CheckerUserIds { get; set; } = new();
        /// <summary>Read-only details of the checkers, filled on reads.</summary>
        public List<PlateCheckerDTO> Checkers { get; set; } = new();
    }

    public class PlateCheckerDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Email { get; set; }
    }
}
