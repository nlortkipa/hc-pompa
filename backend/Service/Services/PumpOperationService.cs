using AutoMapper;
using Dal.Interfaces;
using Data.Entities;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class PumpOperationService(IPumpOperationRepository repository, IMapper mapper) : IPumpOperationService
    {
        public async Task<PumpOperationDto> CreateAsync(CreatePumpOperationDto dto, int operatorId)
        {
            var entity = mapper.Map<PumpOperation>(dto);
            entity.OperatorId = operatorId;

            await repository.AddAsync(entity);
            // repository.AddAsync already calls SaveChangesAsync in BaseRepository

            return mapper.Map<PumpOperationDto>(entity);
        }

        public async Task<IEnumerable<PumpOperationDto>> GetAllAsync(int userId, string role, PumpOperationFilterDto? filter = null)
        {
            var entities = await repository.GetAllAsync();
            
            if (role != "Admin" && role != "Accountant")
            {
                entities = entities.Where(x => x.OperatorId == userId);
            }
            else if (filter != null)
            {
                if (filter.FromDate.HasValue)
                    entities = entities.Where(x => x.Date >= filter.FromDate.Value);
                if (filter.ToDate.HasValue)
                    entities = entities.Where(x => x.Date <= filter.ToDate.Value);
                if (filter.CompanyId.HasValue)
                    entities = entities.Where(x => x.CompanyId == filter.CompanyId.Value);
                if (filter.AddressId.HasValue)
                    entities = entities.Where(x => x.AddressId == filter.AddressId.Value);
                if (filter.OperatorId.HasValue)
                    entities = entities.Where(x => x.OperatorId == filter.OperatorId.Value);
                if (!string.IsNullOrEmpty(filter.PlateNumber))
                    entities = entities.Where(x => x.PlateNumber == filter.PlateNumber);
            }

            return mapper.Map<IEnumerable<PumpOperationDto>>(entities);
        }

        public async Task<PumpOperationDto?> GetByIdAsync(int id, int userId, string role)
        {
            var entity = await repository.GetByIdAsync(id);
            
            if (entity == null) return null;

            if (role != "Admin" && role != "Accountant" && entity.OperatorId != userId)
            {
                return null;
            }

            return mapper.Map<PumpOperationDto>(entity);
        }
    }
}
