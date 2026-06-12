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
        Task<IEnumerable<PumpOperationDto>> GetAllAsync(int userId, string role, PumpOperationFilterDto? filter = null);
        Task<PumpOperationDto?> GetByIdAsync(int id, int userId, string role);
    }
}
