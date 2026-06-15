using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController(ICompanyService companyService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDTO>>> GetAll()
        {
            var companies = await companyService.GetAllAsync();
            return Ok(companies);
        }

        [Authorize]
        [HttpGet("{id:int}/addresses")]
        public async Task<ActionResult<IEnumerable<AddressDTO>>> GetCompanyAddresses(int id)
        {
            var addresses = await companyService.GetAddressesAsync(id);
            return Ok(addresses);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CompanyDTO>> Create(CreateCompanyDTO dto)
        {
            var company = await companyService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAll), company);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateCompanyDTO dto)
        {
            await companyService.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await companyService.DeleteAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/addresses")]
        public async Task<ActionResult<AddressDTO>> AddAddress(int id, CreateAddressDTO dto)
        {
            var address = await companyService.AddAddressAsync(id, dto);
            return Ok(address);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("addresses/{addressId:int}")]
        public async Task<IActionResult> UpdateAddress(int addressId, UpdateAddressDTO dto)
        {
            await companyService.UpdateAddressAsync(addressId, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}/addresses/{addressId:int}")]
        public async Task<IActionResult> RemoveAddress(int id, int addressId)
        {
            await companyService.RemoveAddressAsync(id, addressId);
            return NoContent();
        }
    }
}
