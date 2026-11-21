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
        /// <summary>Updates the plate and carries the change over to reports that copy plate data.</summary>
        Task<PlateUpdateResultDTO> UpdateAsync(int id, PlateDTO dto);
        Task DeleteAsync(int id);
    }
}
