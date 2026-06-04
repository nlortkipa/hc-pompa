using AutoMapper;
using Dal.Interfaces;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(IMapper mapper, ICompanyRepository companyRepository)
        {
            _mapper = mapper;
            _companyRepository = companyRepository;
        }

        public async Task<IEnumerable<CompanyDTO>> GetAllAsync()
        {
            var companies = await _companyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CompanyDTO>>(companies);
        }

        public async Task<IEnumerable<AddressDTO>> GetAddressesAsync(int id)
        {
            var addresses = await _companyRepository.GetAddressesAsync(id);
            return _mapper.Map<IEnumerable<AddressDTO>>(addresses);
        }
    }
}
