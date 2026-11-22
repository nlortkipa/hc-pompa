using AutoMapper;
using Dal.Interfaces;
using Data.Entities;
using Service.DTOs;
using Service.Exceptions;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
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
        private const int MaxNoteLength = 500;
        private const string NotPendingMessage = "Only pending operations can be changed";

        public async Task<PumpOperationDto> CreateAsync(CreatePumpOperationDto dto, int operatorId)
        {
            var (plate, checkers) = await ResolvePlateAndCheckersAsync(dto.PlateNumber);

            var entity = mapper.Map<PumpOperation>(dto);
            entity.OperatorId = operatorId;
            ApplyPlateRules(entity, dto, plate);
            entity.ReviewStatus = PendingStatus;

            await repository.AddAsync(entity);

            foreach (var request in BuildReviewRequests(checkers))
            {
                request.PumpOperationId = entity.Id;
                await reviewRequestRepository.AddAsync(request);
            }

            var createdEntity = await repository.GetByIdAsync(entity.Id) ?? entity;
            return mapper.Map<PumpOperationDto>(createdEntity);
        }

        public async Task<PumpOperationUpdateResult?> UpdateAsync(int id, CreatePumpOperationDto dto, int operatorId)
        {
            var existing = await repository.GetByIdAsync(id);
            if (existing == null || existing.OperatorId != operatorId) return null;
            EnsurePending(existing);

            var (plate, checkers) = await ResolvePlateAndCheckersAsync(dto.PlateNumber);

            var values = mapper.Map<PumpOperation>(dto);
            ApplyPlateRules(values, dto, plate);

            var updated = await repository.TryUpdatePendingAsync(id, operatorId, values, BuildReviewRequests(checkers));
            if (updated == null)
            {
                // A checker decided between our read and the update.
                throw new ConflictException(NotPendingMessage);
            }

            var addedIds = updated.Value.AddedRequests.Select(r => r.Id).ToHashSet();
            var kept = (await reviewRequestRepository.GetByPumpOperationIdAsync(id))
                .Where(r => !addedIds.Contains(r.Id))
                .Select(MapLinkDto)
                .ToList();

            return new PumpOperationUpdateResult
            {
                Operation = mapper.Map<PumpOperationDto>(updated.Value.Operation),
                AddedReviewRequests = updated.Value.AddedRequests.Select(MapLinkDto).ToList(),
                KeptReviewRequests = kept
            };
        }

        public async Task<PumpOperationDeleteResult?> DeleteAsync(int id, int operatorId)
        {
            var existing = await repository.GetByIdAsync(id);
            if (existing == null || existing.OperatorId != operatorId) return null;
            EnsurePending(existing);

            // Read before deleting: the requests are cascade-deleted with the operation.
            var operation = mapper.Map<PumpOperationDto>(existing);
            var requests = (await reviewRequestRepository.GetByPumpOperationIdAsync(id))
                .Select(MapLinkDto)
                .ToList();

            if (!await repository.TryDeletePendingAsync(id, operatorId))
            {
                throw new ConflictException(NotPendingMessage);
            }

            return new PumpOperationDeleteResult { Operation = operation, ReviewRequests = requests };
        }

        private static void EnsurePending(PumpOperation operation)
        {
            if (operation.ReviewStatus != PendingStatus)
            {
                throw new ConflictException(NotPendingMessage);
            }
        }

        private async Task<(Plate Plate, List<User> Checkers)> ResolvePlateAndCheckersAsync(string plateNumber)
        {
            var plate = (await plateRepository.GetAllAsync())
                .FirstOrDefault(p => p.Number.Equals(plateNumber, StringComparison.OrdinalIgnoreCase));

            if (plate == null)
            {
                throw new InvalidOperationException("Plate was not found");
            }

            var checkers = plate.Checkers
                .Where(checker => checker.User != null)
                .GroupBy(checker => checker.UserId)
                .Select(group => group.First().User)
                .ToList();

            if (checkers.Count == 0)
            {
                throw new InvalidOperationException("At least one checker is required for this plate");
            }

            return (plate, checkers);
        }

        private static void ApplyPlateRules(PumpOperation entity, CreatePumpOperationDto dto, Plate plate)
        {
            entity.PlateNumber = plate.Number;
            entity.PumpType = plate.Type;
            entity.DistanceRange = null;
            entity.Note = NormalizeNote(dto.Note);
            if (dto.IsIdle)
            {
                entity.ConcreteVolume = 0;
                entity.AdditionalStanding = 0;
                entity.DismantleAssemble = 0;
            }
            else if (!plate.IsStationary)
            {
                entity.DismantleAssemble = 0;
            }
        }

        private static List<PumpOperationReviewRequest> BuildReviewRequests(IEnumerable<User> checkers)
        {
            return checkers.Select(checker => new PumpOperationReviewRequest
            {
                CheckerUserId = checker.Id,
                // Snapshot of the address the link is mailed to; null when the user has no email.
                CheckerGmail = string.IsNullOrWhiteSpace(checker.Email) ? null : checker.Email,
                Token = Guid.NewGuid().ToString("N"),
                Status = PendingStatus,
                CreatedAt = DateTime.UtcNow
            }).ToList();
        }

        public async Task<IEnumerable<PumpOperationDto>> GetAllAsync(int userId, string role, PumpOperationFilterDto? filter = null)
        {
            var entities = await repository.GetAllAsync();
            
            if (role != "Admin" && role != "Accountant")
            {
                entities = entities.Where(x => x.OperatorId == userId);
            }

            if (filter != null)
            {
                if (filter.FromDate.HasValue)
                    entities = entities.Where(x => x.Date.Date >= filter.FromDate.Value.Date);
                if (filter.ToDate.HasValue)
                    entities = entities.Where(x => x.Date.Date <= filter.ToDate.Value.Date);
                if (filter.CompanyId.HasValue)
                    entities = entities.Where(x => x.CompanyId == filter.CompanyId.Value);
                if (!string.IsNullOrWhiteSpace(filter.CompanyName))
                    entities = entities.Where(x => x.Company.Name.Contains(filter.CompanyName.Trim(), StringComparison.OrdinalIgnoreCase));
                if (filter.AddressId.HasValue)
                    entities = entities.Where(x => x.AddressId == filter.AddressId.Value);
                if (filter.OperatorId.HasValue)
                    entities = entities.Where(x => x.OperatorId == filter.OperatorId.Value);
                if (!string.IsNullOrWhiteSpace(filter.OperatorName))
                {
                    var operatorName = filter.OperatorName.Trim();
                    entities = entities.Where(x =>
                        $"{x.Operator.FirstName} {x.Operator.LastName}"
                            .Contains(operatorName, StringComparison.OrdinalIgnoreCase));
                }
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

            return requests.Select(MapLinkDto);
        }

        public async Task<IEnumerable<PumpOperationReviewDto>> GetReviewsForCheckerAsync(int checkerUserId)
        {
            var requests = await reviewRequestRepository.GetByCheckerUserIdAsync(checkerUserId);

            return requests
                .OrderByDescending(r => r.PumpOperation.ReviewStatus == PendingStatus)
                .ThenByDescending(r => r.PumpOperation.Date)
                .ThenByDescending(r => r.PumpOperationId)
                .Select(MapReviewDto)
                .ToList();
        }

        public async Task<PumpOperationReviewDto?> GetReviewByTokenAsync(string token)
        {
            var request = await reviewRequestRepository.GetByTokenAsync(token);
            return request == null ? null : MapReviewDto(request);
        }

        public async Task<PumpOperationReviewDto?> SubmitReviewDecisionAsync(string token, string decision, string? expectedVersion = null)
        {
            if (decision != AcceptedStatus && decision != RejectedStatus)
            {
                throw new Exception("Review decision is not valid");
            }

            var request = await reviewRequestRepository.GetByTokenAsync(token);
            if (request == null) return null;

            if (request.PumpOperation.ReviewStatus == PendingStatus)
            {
                var checkVersion = !string.IsNullOrEmpty(expectedVersion);
                if (checkVersion && ComputeVersion(request.PumpOperation) != expectedVersion)
                {
                    throw new ReviewOutdatedException(MapReviewDto(request));
                }

                var decided = await reviewRequestRepository.TrySetFirstDecisionAsync(
                    request.PumpOperationId,
                    token,
                    decision,
                    GetReviewerLabel(request),
                    DateTime.UtcNow,
                    // Guards the gap between the version check above and the update.
                    checkVersion ? request.PumpOperation : null);

                request = await reviewRequestRepository.GetByTokenAsync(token);
                if (request == null) return null;

                if (!decided && checkVersion && request.PumpOperation.ReviewStatus == PendingStatus)
                {
                    // Still pending, so the guard failed because the operator edited it just now.
                    throw new ReviewOutdatedException(MapReviewDto(request));
                }
            }

            return MapReviewDto(request);
        }

        /// <summary>Short hash of the editable fields, used to detect edits between load and decision.</summary>
        private static string ComputeVersion(PumpOperation operation)
        {
            var parts = string.Join("|",
                operation.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                operation.CompanyId,
                operation.AddressId,
                operation.ConcreteVolume.ToString("0.############", CultureInfo.InvariantCulture),
                operation.AdditionalStanding,
                operation.IsIdle,
                operation.IsDaily,
                operation.DismantleAssemble,
                operation.PlateNumber,
                operation.PumpType,
                operation.DistanceRange ?? "",
                operation.Note ?? "");

            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(parts));
            return Convert.ToHexString(hash, 0, 8).ToLowerInvariant();
        }

        private static PumpOperationReviewRequestLinkDto MapLinkDto(PumpOperationReviewRequest r) => new()
        {
            Id = r.Id,
            PumpOperationId = r.PumpOperationId,
            Token = r.Token,
            Status = r.Status,
            CheckerUserId = r.CheckerUserId,
            CheckerGmail = r.CheckerGmail,
            CreatedAt = r.CreatedAt,
            ReviewedAt = r.ReviewedAt
        };

        private static PumpOperationReviewDto MapReviewDto(PumpOperationReviewRequest request)
        {
            var operation = request.PumpOperation;

            return new PumpOperationReviewDto
            {
                Token = request.Token,
                Version = ComputeVersion(operation),
                ReviewStatus = operation.ReviewStatus,
                ReviewedAt = operation.ReviewedAt,
                ReviewedByCheckerGmail = operation.ReviewedByCheckerGmail,
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
                AdditionalStanding = operation.AdditionalStanding,
                IsIdle = operation.IsIdle,
                IsDaily = operation.IsDaily,
                DismantleAssemble = operation.DismantleAssemble,
                PlateNumber = operation.PlateNumber,
                PumpType = operation.PumpType,
                DistanceRange = operation.DistanceRange,
                Note = operation.Note
            };
        }

        private static string GetReviewerLabel(PumpOperationReviewRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.CheckerUser?.Email))
            {
                return request.CheckerUser.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.CheckerGmail))
            {
                return request.CheckerGmail;
            }

            if (request.CheckerUser != null)
            {
                return $"{request.CheckerUser.FirstName} {request.CheckerUser.LastName}".Trim();
            }

            return "—";
        }

        private static string? NormalizeNote(string? note)
        {
            if (string.IsNullOrWhiteSpace(note))
            {
                return null;
            }

            var trimmed = note.Trim();
            if (trimmed.Length > MaxNoteLength)
            {
                throw new InvalidOperationException($"Note cannot be longer than {MaxNoteLength} characters");
            }

            return trimmed;
        }

    }
}
