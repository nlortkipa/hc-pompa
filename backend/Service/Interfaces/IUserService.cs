using Dal.Interfaces;
using Data.Entities;
using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO?> GetByIdAsync(int id);
        Task<UserDTO?> GetByNumberAsync(string number);
    }
}
