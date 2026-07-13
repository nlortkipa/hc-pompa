using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Helpers;
using Service.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly ISmsCodeService _smsCodeService;

        public AuthController(IConfiguration configuration, IUserService userService, ISmsCodeService smsCodeService)
        {
            _configuration = configuration;
            _userService = userService;
            _smsCodeService = smsCodeService;
        }

        [AllowAnonymous]
        [HttpPost("Send-Code")]
        public async Task<ActionResult<SmsCodeDTO>> SendCode([FromBody] string number)
        {
            number = number.Trim();

            if (string.IsNullOrEmpty(number)) 
                return BadRequest("შეიყვანეთ ტელეფონის ნომერი");

            var user = await _userService.GetByNumberAsync(number);
            if (user == null)
                return NotFound("ნომერი ვერ მოიძებნა");

            var smsCode = await _smsCodeService.SendCodeAsync(user.Id, user.PhoneNumber);
            return Ok(smsCode);
        }

        [AllowAnonymous]
        [HttpPost("Verify-Code")]
        public async Task<ActionResult<string>> VerifyCode([FromBody] VerifySmsCodeDTO verifySmsCode)
        {
            var user = await _userService.GetByIdAsync(verifySmsCode.UserId);
            if (user == null)
                return BadRequest("ნომერი ვერ მოიძებნა");

            var res = await _smsCodeService.VerifyCodeAsync(verifySmsCode);
            if (!res) 
                return BadRequest("შეყვანილია ვადაგასული ან არასწორი კოდი");

            var token = TokenHelper.GenerateToken(user.Id, user.PhoneNumber, user.Role, user.TokenVersion, _configuration);
            return Ok(token);
        }
    }
}
