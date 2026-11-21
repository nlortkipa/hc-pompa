using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    [Table("PlateCheckers")]
    public class PlateChecker
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PlateId { get; set; }
        [Required]
        public int UserId { get; set; }

        public Plate Plate { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
