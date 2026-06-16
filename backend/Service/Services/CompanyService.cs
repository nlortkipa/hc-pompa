using AutoMapper;
using Dal.Interfaces;
using Data.Entities;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Service.Services
{
    public class CompanyService(
        ICompanyRepository companyRepository,
        IAddressRepository addressRepository,
        ICompanyAddressRepository companyAddressRepository,
        IMapper mapper) : ICompanyService
    {
        public async Task<IEnumerable<CompanyDTO>> GetAllAsync()
        {
            var companies = await companyRepository.GetAllAsync();
            return mapper.Map<IEnumerable<CompanyDTO>>(companies);
        }

        public async Task<IEnumerable<AddressDTO>> GetAddressesAsync(int id)
        {
            var addresses = await companyRepository.GetAddressesAsync(id);
            return mapper.Map<IEnumerable<AddressDTO>>(addresses);
        }

        public async Task<CompanyDTO> CreateAsync(CreateCompanyDTO dto)
        {
            var existing = await companyRepository.GetAllAsync();
            if (existing.Any(c => c.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Service.Exceptions.ConflictException("Company with this name already exists");
            }
            
            var entity = mapper.Map<Company>(dto);
            await companyRepository.AddAsync(entity);
            return mapper.Map<CompanyDTO>(entity);
        }

        public async Task UpdateAsync(int id, UpdateCompanyDTO dto)
        {
            var entity = await companyRepository.GetByIdAsync(id);
            if (entity != null)
            {
                mapper.Map(dto, entity);
                await companyRepository.UpdateAsync(entity);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await companyRepository.DeleteAsync(id);
        }

        public async Task<AddressDTO> AddAddressAsync(int companyId, CreateAddressDTO dto)
        {
            var address = mapper.Map<Address>(dto);
            await addressRepository.AddAsync(address);

            await companyAddressRepository.AddAsync(new CompanyAddress
            {
                CompanyId = companyId,
                AddressId = address.Id
            });

            return mapper.Map<AddressDTO>(address);
        }

        public async Task UpdateAddressAsync(int addressId, UpdateAddressDTO dto)
        {
            var entity = await addressRepository.GetByIdAsync(addressId);
            if (entity != null)
            {
                mapper.Map(dto, entity);
                await addressRepository.UpdateAsync(entity);
            }
        }

        public async Task RemoveAddressAsync(int companyId, int addressId)
        {
            // We only remove the association, but if address is no longer used, we might want to delete it.
            // For now, let's delete the association.
            await companyAddressRepository.DeleteByAddressIdAsync(addressId);
            await addressRepository.DeleteAsync(addressId);
        }
    }
}
