using Data.Entities;
using Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dal.Interfaces
{
    public interface ISmsCodeRepository : IBaseRepository<SmsCode>
    {
        Task<IEnumerable<SmsCode>> GetAllByNumberAsync(string number);
        Task<SmsCode?> GetByUserIdAsync(int userId);
    }
}
