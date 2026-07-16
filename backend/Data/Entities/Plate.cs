using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entities
{
    public class Plate
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public string Type { get; set; } = string.Empty;
        public ICollection<PlateCheckerEmail> CheckerEmails { get; set; } = new List<PlateCheckerEmail>();
    }
}
