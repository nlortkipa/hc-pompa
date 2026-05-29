using Data.Entities;
using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interfaces
{
    public interface ISmsCodeService
    {
        Task<IEnumerable<SmsCodeDTO>> GetAllByNumberAsync(string number);
        Task<SmsCodeDTO> SendCodeAsync(int userId, string phoneNumber);
    }
}
