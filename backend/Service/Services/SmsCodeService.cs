using AutoMapper;
using Dal.Interfaces;
using Data.Entities;
using Microsoft.Extensions.Configuration;
using Service.DTOs;
using Service.Interfaces;

namespace Service.Services
{
    public class SmsCodeService : ISmsCodeService
    {
        private readonly string? _smsKey;
        private readonly bool _sendSms;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly ISmsCodeRepository _smsCodeRepository;

        public SmsCodeService(HttpClient httpClient, IMapper mapper, ISmsCodeRepository smsCodeRepository, IConfiguration configuration)
        {
            _smsKey = Environment.GetEnvironmentVariable("SMS_KEY");
            _sendSms = !bool.TryParse(configuration["Sms:SendSms"], out var sendSms) || sendSms;

            if (_sendSms && string.IsNullOrWhiteSpace(_smsKey))
                throw new Exception("SMS_KEY Not Set.");

            _httpClient = httpClient;
            _mapper = mapper;
            _smsCodeRepository = smsCodeRepository;
        }

        public async Task<IEnumerable<SmsCodeDTO>> GetAllByNumberAsync(string number)
        {
            var smsCodes = await _smsCodeRepository.GetAllByNumberAsync(number);
            return _mapper.Map<IEnumerable<SmsCodeDTO>>(smsCodes);
        }

        public async Task<SmsCodeDTO> SendCodeAsync(int userId, string phoneNumber)
        {
            var code = Random.Shared.Next(1000, 9999).ToString();

            var existing = await _smsCodeRepository.GetByUserIdAsync(userId);
            if (existing != null)
                await _smsCodeRepository.DeleteAsync(existing.Id);

            var smsCode = new SmsCode
            {
                UserId = userId,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };

            await _smsCodeRepository.AddAsync(smsCode);

            if (_sendSms)
                await SendSmsAsync(phoneNumber, code);
            else
                Console.WriteLine($"[SMS DEV] {phoneNumber} -> {code}");

            var smsCodeModel = _mapper.Map<SmsCodeDTO>(smsCode);
            smsCodeModel.UserNumber = phoneNumber;
            return smsCodeModel;
        }

        public async Task<bool> VerifyCodeAsync(VerifySmsCodeDTO model)
        {
            var code = await _smsCodeRepository.GetByUserIdAsync(model.UserId);
            if (code == null)
                return false;

            if (code.ExpiresAt < DateTime.UtcNow)
                return false;

            return code.Code == model.Code;
        }

        public async Task SendMessageAsync(string phoneNumber, string message)
        {
            if (_sendSms)
                await SendSmsAsync(phoneNumber, message);
            else
                Console.WriteLine($"[SMS DEV] {phoneNumber} -> {message}");
        }

        private async Task SendSmsAsync(string phoneNumber, string code)
        {
            if (string.IsNullOrWhiteSpace(_smsKey))
                throw new Exception("SMS_KEY Not Set.");

            try
            {
                var queryParameters = $"key={Encode(_smsKey)}" +
                                      $"&destination={Encode(phoneNumber)}" +
                                      $"&sender={Encode("Beton.ge")}" +
                                      $"&content={Encode(code)}";

                string baseUrl = "http://smsoffice.ge/api/v2/send";
                string fullUrl = $"{baseUrl}?{queryParameters}";

                HttpResponseMessage response = await _httpClient.GetAsync(fullUrl);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error sending SMS: {ex.Message}");
                throw;
            }
        }

        private static string Encode(string value)
        {
            return Uri.EscapeDataString(value);
        }
    }
}
