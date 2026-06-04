using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Service.DTOs
{
    public class CompanyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class CreateCompanyDTO
    {
        public string Name { get; set; } = null!;
    }

    public class UpdateCompanyDTO
    {
        public string Name { get; set; } = null!;
    }
}
