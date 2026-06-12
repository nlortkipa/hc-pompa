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
        public int CompanyId { get; set; }
        public int AddressId { get; set; }
        public decimal ConcreteVolume { get; set; }
        public int PumpRelocation { get; set; }
        public int StationaryRelocation { get; set; }
        public int HydraulicRelocation { get; set; }
        public int StationaryReposition { get; set; }
        public decimal IdleTime { get; set; }
        public int DismantleAssemble { get; set; }
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
    }
}
