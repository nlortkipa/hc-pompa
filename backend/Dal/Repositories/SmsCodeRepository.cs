using Dal.Interfaces;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dal.Repositories
{
    public class SmsCodeRepository : BaseRepository<SmsCode>, ISmsCodeRepository
    {
        private readonly ProjectContext _context;

        public SmsCodeRepository(ProjectContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SmsCode>> GetAllByNumberAsync(string number)
        {
            return await _context.Set<SmsCode>()
                .Include(x => x.User)
                .Where(x => x.User.Number == number)
                .ToListAsync();
        }

        public async Task<SmsCode?> GetByUserIdAsync(int userId)
        {
            return await _context.SmsCodes
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
