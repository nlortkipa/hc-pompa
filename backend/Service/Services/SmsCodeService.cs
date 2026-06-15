using AutoMapper;
using Dal.Interfaces;
using Dal.Repositories;
using Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace Service.Services
{
    public class SmsCodeService : ISmsCodeService
    {
        private readonly string _smsKey;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly ISmsCodeRepository _smsCodeRepository;

        public SmsCodeService(HttpClient httpClient, IMapper mapper, ISmsCodeRepository smsCodeRepository)
        {
            _smsKey = Environment.GetEnvironmentVariable("SMS_KEY")
                ?? throw new Exception("SMS_KEY Not Set.");
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
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            await _smsCodeRepository.AddAsync(smsCode);

            Console.WriteLine($"[SMS SEND] {phoneNumber} -> {code}");
            try
            {
                var queryParameters = $"key={HttpUtility.UrlEncode(_smsKey)}" +
                                      $"&destination={HttpUtility.UrlEncode(phoneNumber)}" +
                                      $"&sender={HttpUtility.UrlEncode("Beton.ge")}" +
                                      $"&content={HttpUtility.UrlEncode(code)}";

                string baseUrl = "http://smsoffice.ge/api/v2/send";
                string fullUrl = $"{baseUrl}?{queryParameters}";

                HttpResponseMessage response = await _httpClient.GetAsync(fullUrl);

                response.EnsureSuccessStatusCode();
                string responseData = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error sending SMS: {ex.Message}");
                throw;
            }

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
    }
}
