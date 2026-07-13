using AutoMapper;
using Dal.Interfaces;
using Data.Entities;
using Service.DTOs;
using Service.Exceptions;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class PlateService : IPlateService
    {
        private readonly IPlateRepository _plateRepository;
        private readonly IMapper _mapper;

        public PlateService(IPlateRepository plateRepository, IMapper mapper)
        {
            _plateRepository = plateRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PlateDTO>> GetAllAsync()
        {
            var entities = await _plateRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PlateDTO>>(entities);
        }

        public async Task<PlateDTO> GetByIdAsync(int id)
        {
            var entity = await _plateRepository.GetByIdAsync(id);
            return _mapper.Map<PlateDTO>(entity);
        }

        public async Task<PlateDTO> CreateAsync(PlateDTO dto)
        {
            Normalize(dto);
            Validate(dto);

            var allPlates = await _plateRepository.GetAllAsync();
            if (allPlates.Any(p => p.Number.Equals(dto.Number, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ConflictException("სანომრე ნიშანი უკვე არსებობს");
            }

            var entity = _mapper.Map<Plate>(dto);
            await _plateRepository.AddAsync(entity);
            return _mapper.Map<PlateDTO>(entity);
        }

        public async Task UpdateAsync(int id, PlateDTO dto)
        {
            Normalize(dto);
            Validate(dto);

            var entity = await _plateRepository.GetByIdAsync(id);
            if (entity == null) throw new Exception("Plate not found");

            var allPlates = await _plateRepository.GetAllAsync();
            if (allPlates.Any(p => p.Id != id && p.Number.Equals(dto.Number, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ConflictException("სანომრე ნიშანი უკვე არსებობს");
            }

            _mapper.Map(dto, entity);
            await _plateRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _plateRepository.DeleteAsync(id);
        }

        private static void Normalize(PlateDTO dto)
        {
            dto.Number = dto.Number?.Trim().ToUpperInvariant() ?? string.Empty;
            dto.CheckerGmail = dto.CheckerGmail?.Trim().ToLowerInvariant() ?? string.Empty;
        }

        private static void Validate(PlateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Number))
            {
                throw new Exception("Plate number is required");
            }

            if (string.IsNullOrWhiteSpace(dto.CheckerGmail))
            {
                throw new Exception("Checker Gmail is required");
            }

            if (!dto.CheckerGmail.Contains('@') || !dto.CheckerGmail.Contains('.'))
            {
                throw new Exception("Checker Gmail is not valid");
            }
        }
    }
}
