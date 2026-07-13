using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO?> GetByIdAsync(int id);
        Task<UserDTO?> GetByNumberAsync(string number);
        Task<IEnumerable<UserDTO>> GetAllAsync();
        Task<UserDTO> CreateAsync(CreateUserDTO dto);
        Task<UserDTO> UpdateAsync(int id, UpdateUserDTO dto, int currentUserId);
        Task<UserDTO> UpdateProfileAsync(int id, UpdateProfileDTO dto);
        Task LogoutAllDevicesAsync(int id);
        Task DeleteAsync(int id, int currentUserId);
    }
}
