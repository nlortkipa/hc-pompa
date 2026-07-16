using System;

namespace Service.DTOs
{
    public class PumpOperationFilterDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public int? AddressId { get; set; }
        public int? OperatorId { get; set; }
        public string? OperatorName { get; set; }
        public string? PlateNumber { get; set; }
    }
}
