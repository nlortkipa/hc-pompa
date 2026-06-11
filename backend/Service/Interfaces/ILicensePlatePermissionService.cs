using Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interfaces
{
    public interface ILicensePlatePermissionService
    {
        Task<LicensePlatePermission?> GetByLicensePlateAsync(string licensePlate);
    }
}
