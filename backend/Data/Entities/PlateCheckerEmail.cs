using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    [Table("PlateCheckerEmails")]
    public class PlateCheckerEmail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PlateId { get; set; }
        [Required]
        public string Email { get; set; } = null!;

        public Plate Plate { get; set; } = null!;
    }
}
