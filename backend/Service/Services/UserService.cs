using AutoMapper;
using Dal.Interfaces;
using Data.Entities;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Linq;

namespace Service.Services
{
    public class UserService(
        IMapper mapper,
        IUserRepository userRepository,
        IPumpOperationReviewRequestRepository reviewRequestRepository) : IUserService
    {
        private const string AdminRole = "Admin";
        private const int MaxUsernameLength = 75;
        private const int MaxAvatarUrlLength = 2_100_000;
        private const int MaxEmailLength = 120;

        public async Task<UserDTO?> GetByIdAsync(int id)
        {
            var user = await userRepository.GetByIdAsync(id);
            return mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO?> GetByNumberAsync(string number)
        {
            var user = await userRepository.GetByNumberAsync(number);
            return mapper.Map<UserDTO>(user);
        }

        public async Task<IEnumerable<UserDTO>> GetAllAsync()
        {
            var users = await userRepository.GetAllAsync();
            return mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO> CreateAsync(CreateUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || 
                string.IsNullOrWhiteSpace(dto.LastName) || 
                string.IsNullOrWhiteSpace(dto.PhoneNumber) || 
                string.IsNullOrWhiteSpace(dto.PersonalId))
            {
                throw new Exception("All fields are required");
            }

            dto.PhoneNumber = dto.PhoneNumber.Replace(" ", "");

            if (dto.PersonalId.Trim().Length != 11)
            {
                throw new Exception("Personal ID must be exactly 11 characters");
            }

            var allUsers = await userRepository.GetAllAsync();
            if (allUsers.Any(u => u.PhoneNumber == dto.PhoneNumber))
            {
                throw new Service.Exceptions.ConflictException("User with this phone number already exists");
            }
            if (allUsers.Any(u => u.PersonalId == dto.PersonalId))
            {
                throw new Service.Exceptions.ConflictException("User with this personal ID already exists");
            }

            var user = mapper.Map<User>(dto);
            user.Email = NormalizeEmail(dto.Email);
            user.CreatedAt = DateTime.UtcNow;
            user.Username = NormalizeUsername(dto.Username, dto.FirstName, dto.LastName);
            user.AvatarUrl = NormalizeAvatarUrl(dto.AvatarUrl);
            user.TokenVersion = 0;

            await userRepository.AddAsync(user);
            return mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> UpdateAsync(int id, UpdateUserDTO dto, int currentUserId)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || 
                string.IsNullOrWhiteSpace(dto.LastName) || 
                string.IsNullOrWhiteSpace(dto.PhoneNumber) || 
                string.IsNullOrWhiteSpace(dto.PersonalId))
            {
                throw new Exception("All fields are required");
            }

            dto.PhoneNumber = dto.PhoneNumber.Replace(" ", "");

            if (dto.PersonalId.Trim().Length != 11)
            {
                throw new Exception("Personal ID must be exactly 11 characters");
            }

            var user = await userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            if (IsAdmin(user.Role) && !IsAdmin(dto.Role) && await IsOnlyAdminAsync(user.Id))
            {
                var message = id == currentUserId
                    ? "You cannot change your own role because you are the only remaining admin."
                    : "You cannot remove the only remaining admin role.";
                throw new Service.Exceptions.ConflictException(message);
            }

            var previousEmail = user.Email;
            mapper.Map(dto, user);
            user.Email = NormalizeEmail(dto.Email);
            await userRepository.UpdateAsync(user);

            if (!string.Equals(previousEmail, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                // Review requests keep a copy of the checker's address; keep pending ones current.
                await reviewRequestRepository.UpdatePendingCheckerEmailAsync(user.Id, user.Email);
            }
            
            return mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> UpdateProfileAsync(int id, UpdateProfileDTO dto)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            user.Username = NormalizeUsername(dto.Username, user.FirstName, user.LastName);
            user.AvatarUrl = NormalizeAvatarUrl(dto.AvatarUrl);

            await userRepository.UpdateAsync(user);
            return mapper.Map<UserDTO>(user);
        }

        public async Task LogoutAllDevicesAsync(int id)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            user.TokenVersion++;
            await userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(int id, int currentUserId)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            if (IsAdmin(user.Role) && await IsOnlyAdminAsync(user.Id))
            {
                var message = id == currentUserId
                    ? "You cannot delete yourself because you are the only remaining admin."
                    : "You cannot delete the only remaining admin.";
                throw new Service.Exceptions.ConflictException(message);
            }

            await userRepository.DeleteAsync(id);
        }

        private async Task<bool> IsOnlyAdminAsync(int userId)
        {
            var users = await userRepository.GetAllAsync();
            return users.Count(u => u.Id != userId && IsAdmin(u.Role)) == 0;
        }

        private static bool IsAdmin(string? role)
        {
            return string.Equals(role, AdminRole, StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeUsername(string? username, string firstName, string lastName)
        {
            var value = string.IsNullOrWhiteSpace(username)
                ? $"{firstName} {lastName}".Trim()
                : username.Trim();

            if (value.Length > MaxUsernameLength)
            {
                throw new Exception($"Username must be {MaxUsernameLength} characters or fewer");
            }

            return value;
        }

        private static string? NormalizeEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var value = email.Trim().ToLowerInvariant();
            if (value.Length > MaxEmailLength || !new EmailAddressAttribute().IsValid(value))
            {
                throw new Exception("Email is not valid");
            }

            return value;
        }

        private static string? NormalizeAvatarUrl(string? avatarUrl)
        {
            if (string.IsNullOrWhiteSpace(avatarUrl))
            {
                return null;
            }

            var value = avatarUrl.Trim();
            if (value.Length > MaxAvatarUrlLength)
            {
                throw new Exception("Avatar image is too large");
            }

            if (!value.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase) &&
                !Uri.TryCreate(value, UriKind.Absolute, out _))
            {
                throw new Exception("Avatar must be an image data URL or an absolute URL");
            }

            return value;
        }
    }
}
