using Dal.Interfaces;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Repositories
{
    public class PlateRepository : BaseRepository<Plate>, IPlateRepository
    {
        private readonly ProjectContext _context;

        public PlateRepository(ProjectContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<IEnumerable<Plate>> GetAllAsync()
        {
            return await _context.Plates
                .Include(x => x.Checkers)
                    .ThenInclude(c => c.User)
                .ToListAsync();
        }

        public override async Task<Plate?> GetByIdAsync(int id)
        {
            return await _context.Plates
                .Include(x => x.Checkers)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
