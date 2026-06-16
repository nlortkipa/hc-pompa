using AutoMapper;
using Dal.Interfaces;
using Data.Entities;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Service.Services
{
    public class UserService(IMapper mapper, IUserRepository userRepository) : IUserService
    {
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
            user.CreatedAt = DateTime.Now;

            await userRepository.AddAsync(user);
            return mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> UpdateAsync(int id, UpdateUserDTO dto)
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

            mapper.Map(dto, user);
            await userRepository.UpdateAsync(user);
            
            return mapper.Map<UserDTO>(user);
        }

        public async Task DeleteAsync(int id)
        {
            await userRepository.DeleteAsync(id);
        }
    }
}
