using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PumpOperationsController(IPumpOperationService service) : ControllerBase
    {
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<ActionResult<PumpOperationDto>> Create(CreatePumpOperationDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var result = await service.CreateAsync(dto, userId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PumpOperationDto>>> GetAll([FromQuery] PumpOperationFilterDto? filter)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "User";

            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var result = await service.GetAllAsync(userId, role, filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PumpOperationDto>> GetById(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "User";

            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var result = await service.GetByIdAsync(id, userId, role);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
