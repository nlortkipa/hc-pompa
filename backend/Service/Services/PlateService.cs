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
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public PlateService(IPlateRepository plateRepository, IUserRepository userRepository, IMapper mapper)
        {
            _plateRepository = plateRepository;
            _userRepository = userRepository;
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
            await EnsureCheckersExistAsync(dto.CheckerUserIds);

            var allPlates = await _plateRepository.GetAllAsync();
            if (allPlates.Any(p => p.Number.Equals(dto.Number, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ConflictException("სანომრე ნიშანი უკვე არსებობს");
            }

            var entity = _mapper.Map<Plate>(dto);
            entity.Checkers = dto.CheckerUserIds
                .Select(userId => new PlateChecker { UserId = userId })
                .ToList();
            await _plateRepository.AddAsync(entity);
            return _mapper.Map<PlateDTO>(await _plateRepository.GetByIdAsync(entity.Id) ?? entity);
        }

        public async Task UpdateAsync(int id, PlateDTO dto)
        {
            Normalize(dto);
            Validate(dto);
            await EnsureCheckersExistAsync(dto.CheckerUserIds);

            var entity = await _plateRepository.GetByIdAsync(id);
            if (entity == null) throw new Exception("Plate not found");

            var allPlates = await _plateRepository.GetAllAsync();
            if (allPlates.Any(p => p.Id != id && p.Number.Equals(dto.Number, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ConflictException("სანომრე ნიშანი უკვე არსებობს");
            }

            _mapper.Map(dto, entity);
            var requestedUserIds = dto.CheckerUserIds.ToHashSet();
            var removedCheckers = entity.Checkers
                .Where(x => !requestedUserIds.Contains(x.UserId))
                .ToList();

            foreach (var removedChecker in removedCheckers)
            {
                entity.Checkers.Remove(removedChecker);
            }

            var existingUserIds = entity.Checkers
                .Select(x => x.UserId)
                .ToHashSet();

            foreach (var userId in dto.CheckerUserIds.Where(userId => !existingUserIds.Contains(userId)))
            {
                entity.Checkers.Add(new PlateChecker
                {
                    PlateId = entity.Id,
                    UserId = userId
                });
            }
            await _plateRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _plateRepository.DeleteAsync(id);
        }

        private static void Normalize(PlateDTO dto)
        {
            dto.Number = dto.Number?.Trim().ToUpperInvariant() ?? string.Empty;
            dto.Type = dto.Type?.Trim() ?? string.Empty;
            dto.CheckerUserIds = (dto.CheckerUserIds ?? new List<int>())
                .Where(userId => userId > 0)
                .Distinct()
                .ToList();
        }

        private static void Validate(PlateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Number))
            {
                throw new Exception("Plate number is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Type))
            {
                throw new Exception("Pump type is required");
            }

            if (dto.CheckerUserIds.Count == 0)
            {
                throw new Exception("At least one checker is required");
            }
        }

        private async Task EnsureCheckersExistAsync(List<int> userIds)
        {
            var existingIds = (await _userRepository.GetAllAsync())
                .Select(u => u.Id)
                .ToHashSet();

            if (userIds.Any(id => !existingIds.Contains(id)))
            {
                throw new Exception("One or more checkers were not found");
            }
        }
    }
}
