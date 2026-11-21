using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize(Roles = "Admin,Accountant")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService userService, ISmsCodeService smsService, Microsoft.Extensions.Configuration.IConfiguration configuration) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
        {
            var users = await userService.GetAllAsync();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetById(int id)
        {
            var user = await userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Create(CreateUserDTO dto)
        {
            try
            {
                var user = await userService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (Service.Exceptions.ConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDTO>> Update(int id, UpdateUserDTO dto)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            try
            {
                var user = await userService.UpdateAsync(id, dto, currentUserId.Value);
                return Ok(user);
            }
            catch (Service.Exceptions.ConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            try
            {
                await userService.DeleteAsync(id, currentUserId.Value);
                return NoContent();
            }
            catch (Service.Exceptions.ConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/send-link")]
        public async Task<ActionResult> SendLink(int id)
        {
            var user = await userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            if (string.IsNullOrWhiteSpace(user.PhoneNumber)) return BadRequest("მომხმარებელს ტელეფონი არ აქვს.");

            var siteUrl = GetSiteUrl();
            if (siteUrl == null) return BadRequest("საიტის მისამართი ვერ განისაზღვრა.");

            try
            {
                await smsService.SendMessageAsync(user.PhoneNumber, $"პომპის საიტის ბმული: {siteUrl}");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(502, ex.Message);
            }
        }

        private string? GetSiteUrl()
        {
            var configuredUrl = configuration["SiteUrl"];
            if (!string.IsNullOrWhiteSpace(configuredUrl)) return configuredUrl.TrimEnd('/');

            var productionHost = Environment.GetEnvironmentVariable("VERCEL_PROJECT_PRODUCTION_URL");
            if (!string.IsNullOrWhiteSpace(productionHost)) return $"https://{productionHost.TrimEnd('/')}";

            var origin = Request.Headers.Origin.ToString();
            return string.IsNullOrWhiteSpace(origin) ? null : origin.TrimEnd('/');
        }

        private int? GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userId, out var id) ? id : null;
        }
    }
}
