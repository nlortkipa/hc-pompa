using Dal.Interfaces;
using Data;
using Data.Entities;
using Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dal.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly ProjectContext _context;

        public UserRepository(ProjectContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByNumberAsync(string number)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == number);
        }
    }
}
