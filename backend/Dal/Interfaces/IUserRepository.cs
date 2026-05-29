using Data.Entities;
using Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dal.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByNumberAsync(string number);
    }
}
