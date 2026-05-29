using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISmsCodeService _smsCodeService;

        public AuthController(IUserService userService, ISmsCodeService smsCodeService)
        {
            _userService = userService;
            _smsCodeService = smsCodeService;
        }

        [HttpPost("Send-Code")]
        public async Task<ActionResult<SmsCodeDTO>> SendCode([FromBody] string number)
        {
            number = number.Trim();

            if (string.IsNullOrEmpty(number)) 
                return BadRequest("შეიყვანეთ ტელეფონის ნომერი");

            var user = await _userService.GetByNumberAsync(number);
            if (user == null)
                return NotFound("ნომერი ვერ მოიძებნა");

            var smsCode = await _smsCodeService.SendCodeAsync(user.Id, user.Number);
            return Ok(smsCode);
        }
    }
}
