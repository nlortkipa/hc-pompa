using System;
using System.Collections.Generic;
using System.Text;

namespace Service.DTOs
{
    public class PumpOperationDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int OperatorId { get; set; }
        public string OperatorFirstName { get; set; } = null!;
        public string OperatorLastName { get; set; } = null!;
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public int AddressId { get; set; }
        public string AddressName { get; set; } = null!;
        public decimal ConcreteVolume { get; set; }
        public int PumpRelocation { get; set; }
        public int StationaryRelocation { get; set; }
        public int HydraulicRelocation { get; set; }
        public int StationaryReposition { get; set; }
        public decimal IdleTime { get; set; }
        public int DismantleAssemble { get; set; }
        public string PlateNumber { get; set; } = null!;
        public string ReviewStatus { get; set; } = null!;
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedByCheckerGmail { get; set; }
    }
    
    public class CreatePumpOperationDto
    {
        public DateTime Date { get; set; }
        public int CompanyId { get; set; }
        public int AddressId { get; set; }
        public decimal ConcreteVolume { get; set; }
        public int PumpRelocation { get; set; }
        public int StationaryRelocation { get; set; }
        public int HydraulicRelocation { get; set; }
        public int StationaryReposition { get; set; }
        public decimal IdleTime { get; set; }
        public int DismantleAssemble { get; set; }
        public string PlateNumber { get; set; } = null!;
    }
}
