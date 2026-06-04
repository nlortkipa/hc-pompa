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
    }
}
