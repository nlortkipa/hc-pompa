using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IPlateService
    {
        Task<IEnumerable<PlateDTO>> GetAllAsync();
        Task<PlateDTO> GetByIdAsync(int id);
        Task<PlateDTO> CreateAsync(PlateDTO dto);
        Task UpdateAsync(int id, PlateDTO dto);
        Task DeleteAsync(int id);
    }
}
