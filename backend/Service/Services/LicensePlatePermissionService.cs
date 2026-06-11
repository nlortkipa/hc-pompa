using AutoMapper;
using Dal.Interfaces;
using Data.Entities;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Services
{
    public class LicensePlatePermissionService : ILicensePlatePermissionService
    {
        private readonly IMapper _mapper;
        private readonly ILicensePlatePermissionRepository _licensePlatePermissionRepository;

        public LicensePlatePermissionService(IMapper mapper, ILicensePlatePermissionRepository licensePlatePermissionRepository)
        {
            _mapper = mapper;
            _licensePlatePermissionRepository = licensePlatePermissionRepository;
        }

        public async Task<LicensePlatePermission?> GetByLicensePlateAsync(string licensePlate)
        {
            var licensePlatePermission = await _licensePlatePermissionRepository.GetByLicensePlateAsync(licensePlate);
            return _mapper.Map<LicensePlatePermission?>(licensePlatePermission);
        }
    }
}
