using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class PlateDTO
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public string CheckerGmail { get; set; } = null!;
    }
}
