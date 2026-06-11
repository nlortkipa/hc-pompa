using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly ILicensePlatePermissionService _licensePlatePermissionService;

        public PermissionsController(ILicensePlatePermissionService licensePlatePermissionService)
        {
            _licensePlatePermissionService = licensePlatePermissionService;
        }

        [Authorize]
        [HttpGet("{licensePlate}")]
        public async Task<ActionResult<LicensePlatePermissionDTO>> GetByLicensePlate(string licensePlate)
        {
            var licensePlatePermission = await _licensePlatePermissionService.GetByLicensePlateAsync(licensePlate);
            if (licensePlatePermission == null)
                return NotFound();

            return Ok(licensePlatePermission);
        }
    }
}
