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
    public class PumpOperationService(
        IPumpOperationRepository repository,
        IPlateRepository plateRepository,
        IPumpOperationReviewRequestRepository reviewRequestRepository,
        IMapper mapper) : IPumpOperationService
    {
        private const string PendingStatus = "Pending";
        private const string AcceptedStatus = "Accepted";
        private const string RejectedStatus = "Rejected";

        public async Task<PumpOperationDto> CreateAsync(CreatePumpOperationDto dto, int operatorId)
        {
            var plate = (await plateRepository.GetAllAsync())
                .FirstOrDefault(p => p.Number.Equals(dto.PlateNumber, StringComparison.OrdinalIgnoreCase));

            if (plate == null)
            {
                throw new InvalidOperationException("Plate was not found");
            }

            if (string.IsNullOrWhiteSpace(plate.CheckerGmail))
            {
                throw new InvalidOperationException("Checker Gmail is required for this plate");
            }

            var entity = mapper.Map<PumpOperation>(dto);
            entity.OperatorId = operatorId;
            entity.PlateNumber = plate.Number;
            entity.ReviewStatus = PendingStatus;

            await repository.AddAsync(entity);

            await reviewRequestRepository.AddAsync(new PumpOperationReviewRequest
            {
                PumpOperationId = entity.Id,
                CheckerGmail = plate.CheckerGmail,
                Token = Guid.NewGuid().ToString("N"),
                Status = PendingStatus,
                CreatedAt = DateTime.Now
            });

            var createdEntity = await repository.GetByIdAsync(entity.Id) ?? entity;
            return mapper.Map<PumpOperationDto>(createdEntity);
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
                    entities = entities.Where(x => x.Date.Date >= filter.FromDate.Value.Date);
                if (filter.ToDate.HasValue)
                    entities = entities.Where(x => x.Date.Date <= filter.ToDate.Value.Date);
                if (filter.CompanyId.HasValue)
                    entities = entities.Where(x => x.CompanyId == filter.CompanyId.Value);
                if (filter.AddressId.HasValue)
                    entities = entities.Where(x => x.AddressId == filter.AddressId.Value);
                if (filter.OperatorId.HasValue)
                    entities = entities.Where(x => x.OperatorId == filter.OperatorId.Value);
                if (!string.IsNullOrWhiteSpace(filter.PlateNumber))
                    entities = entities.Where(x => x.PlateNumber.Contains(filter.PlateNumber.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            entities = entities
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Id);

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

        public async Task<IEnumerable<PumpOperationReviewRequestLinkDto>> GetReviewRequestsForOperationAsync(int pumpOperationId)
        {
            var requests = await reviewRequestRepository.GetByPumpOperationIdAsync(pumpOperationId);

            return requests.Select(r => new PumpOperationReviewRequestLinkDto
            {
                Id = r.Id,
                PumpOperationId = r.PumpOperationId,
                Token = r.Token,
                Status = r.Status,
                CheckerGmail = r.CheckerGmail,
                CreatedAt = r.CreatedAt,
                ReviewedAt = r.ReviewedAt
            });
        }

        public async Task<PumpOperationReviewDto?> GetReviewByTokenAsync(string token)
        {
            var request = await reviewRequestRepository.GetByTokenAsync(token);
            return request == null ? null : MapReviewDto(request);
        }

        public async Task<PumpOperationReviewDto?> SubmitReviewDecisionAsync(string token, string decision)
        {
            if (decision != AcceptedStatus && decision != RejectedStatus)
            {
                throw new Exception("Review decision is not valid");
            }

            var request = await reviewRequestRepository.GetByTokenAsync(token);
            if (request == null) return null;

            if (request.PumpOperation.ReviewStatus == PendingStatus)
            {
                var reviewedAt = DateTime.Now;

                request.Status = decision;
                request.ReviewedAt = reviewedAt;
                request.PumpOperation.ReviewStatus = decision;
                request.PumpOperation.ReviewedAt = reviewedAt;
                request.PumpOperation.ReviewedByCheckerGmail = request.CheckerGmail;

                await reviewRequestRepository.UpdateAsync(request);
            }

            return MapReviewDto(request);
        }

        private static PumpOperationReviewDto MapReviewDto(PumpOperationReviewRequest request)
        {
            var operation = request.PumpOperation;

            return new PumpOperationReviewDto
            {
                Token = request.Token,
                ReviewStatus = operation.ReviewStatus,
                ReviewedAt = operation.ReviewedAt,
                CheckerGmail = request.CheckerGmail,
                PumpOperationId = operation.Id,
                Date = operation.Date,
                OperatorId = operation.OperatorId,
                OperatorFirstName = operation.Operator.FirstName,
                OperatorLastName = operation.Operator.LastName,
                CompanyId = operation.CompanyId,
                CompanyName = operation.Company.Name,
                AddressId = operation.AddressId,
                AddressName = operation.Address.Name,
                ConcreteVolume = operation.ConcreteVolume,
                PumpRelocation = operation.PumpRelocation,
                StationaryRelocation = operation.StationaryRelocation,
                HydraulicRelocation = operation.HydraulicRelocation,
                StationaryReposition = operation.StationaryReposition,
                IdleTime = operation.IdleTime,
                DismantleAssemble = operation.DismantleAssemble,
                PlateNumber = operation.PlateNumber
            };
        }
    }
}
