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
        private const long MaxExcelFileSize = 10 * 1024 * 1024;

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
            try
            {
                var company = await companyService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetAll), company);
            }
            catch (Service.Exceptions.ConflictException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("import-excel")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MaxExcelFileSize)]
        public async Task<ActionResult<CompanyExcelImportResultDTO>> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Excel ფაილი აუცილებელია." });

            if (file.Length > MaxExcelFileSize)
                return BadRequest(new { message = "Excel ფაილის ზომა 10 მბ-ს არ უნდა აღემატებოდეს." });

            var extension = Path.GetExtension(file.FileName);
            if (!extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(".xlsm", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "მხარდაჭერილია მხოლოდ .xlsx და .xlsm ფორმატის ფაილები." });
            }

            try
            {
                await using var stream = file.OpenReadStream();
                return Ok(await companyService.ImportExcelAsync(stream));
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Service.Exceptions.ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
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
