using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDTO>> GetAllAsync();
        Task<IEnumerable<AddressDTO>> GetAddressesAsync(int id);
        Task<CompanyExcelImportResultDTO> ImportExcelAsync(Stream excelStream);
        
        // Company CRUD
        Task<CompanyDTO> CreateAsync(CreateCompanyDTO dto);
        Task UpdateAsync(int id, UpdateCompanyDTO dto);
        Task DeleteAsync(int id);

        // Address CRUD (scoped to company)
        Task<AddressDTO> AddAddressAsync(int companyId, CreateAddressDTO dto);
        Task UpdateAddressAsync(int addressId, UpdateAddressDTO dto);
        Task RemoveAddressAsync(int companyId, int addressId);
    }
}
