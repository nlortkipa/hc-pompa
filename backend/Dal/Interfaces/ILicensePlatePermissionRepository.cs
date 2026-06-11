using Data.Entities;
using Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dal.Interfaces
{
    public interface ILicensePlatePermissionRepository : IBaseRepository<LicensePlatePermission>
    {
        Task<LicensePlatePermission?> GetByLicensePlateAsync(string licensePlate);
    }
}
