using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Companies : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public Companies(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDTO>>> GetAll()
        {
            var companies = await _companyService.GetAllAsync();
            if (companies.Count() == 0)
                return NotFound("No companies found");

            return Ok(companies);
        }
    }
}
