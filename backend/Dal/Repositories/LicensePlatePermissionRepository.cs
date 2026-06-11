using Dal.Interfaces;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dal.Repositories
{
    public class LicensePlatePermissionRepository : BaseRepository<LicensePlatePermission>, ILicensePlatePermissionRepository
    {
        private readonly ProjectContext _context;

        public LicensePlatePermissionRepository(ProjectContext context) : base(context)
        {
            _context = context;
        }

        public async Task<LicensePlatePermission?> GetByLicensePlateAsync(string licensePlate)
        {
            return await _context.LicensePlatePermissions
                .FirstOrDefaultAsync(x => x.LicensePlate == licensePlate);
        }
    }
}
