using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IPumpOperationService
    {
        Task<PumpOperationDto> CreateAsync(CreatePumpOperationDto dto, int operatorId);
        Task<IEnumerable<PumpOperationDto>> GetAllAsync();
        Task<PumpOperationDto> GetByIdAsync(int id);
    }
}
