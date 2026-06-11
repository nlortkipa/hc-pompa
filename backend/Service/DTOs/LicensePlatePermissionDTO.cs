using System;
using System.Collections.Generic;
using System.Text;

namespace Service.DTOs
{
    public class LicensePlatePermissionDTO
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; } = null!;
        public int Permission { get; set; }
    }

    public class CreateLicensePlatePermissionDTO
    {
        public string LicensePlate { get; set; } = null!;
        public int Permission { get; set; }
    }

    public class UpdateLicensePlatePermissionDTO
    {
        public string LicensePlate { get; set; } = null!;
        public int Permission { get; set; }
    }
}
