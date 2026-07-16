using AutoMapper;
using Dal.Interfaces;
using Data.Entities;
using Service.DTOs;
using Service.Exceptions;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            entity.CheckerEmails = dto.CheckerEmails
                .Select(email => new PlateCheckerEmail { Email = email })
                .ToList();
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
            var requestedEmails = dto.CheckerEmails.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var removedEmails = entity.CheckerEmails
                .Where(x => !requestedEmails.Contains(x.Email))
                .ToList();

            foreach (var removedEmail in removedEmails)
            {
                entity.CheckerEmails.Remove(removedEmail);
            }

            var existingEmails = entity.CheckerEmails
                .Select(x => x.Email)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var email in dto.CheckerEmails.Where(email => !existingEmails.Contains(email)))
            {
                entity.CheckerEmails.Add(new PlateCheckerEmail
                {
                    PlateId = entity.Id,
                    Email = email
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
            dto.CheckerEmails = (dto.CheckerEmails ?? new List<string>())
                .Select(email => email?.Trim().ToLowerInvariant() ?? string.Empty)
                .Where(email => !string.IsNullOrWhiteSpace(email))
                .Distinct(StringComparer.OrdinalIgnoreCase)
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

            if (dto.CheckerEmails.Count == 0)
            {
                throw new Exception("At least one checker email is required");
            }

            if (dto.CheckerEmails.Any(email => !new EmailAddressAttribute().IsValid(email)))
            {
                throw new Exception("One or more checker emails are not valid");
            }
        }
    }
}
