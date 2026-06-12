using AutoMapper;
using Dal.Interfaces;
using Data.Entities;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<PumpOperationDto>> GetAllAsync()
        {
            var entities = await repository.GetAllAsync();
            return mapper.Map<IEnumerable<PumpOperationDto>>(entities);
        }

        public async Task<PumpOperationDto> GetByIdAsync(int id)
        {
            var entity = await repository.GetByIdAsync(id);
            return mapper.Map<PumpOperationDto>(entity);
        }
    }
}
