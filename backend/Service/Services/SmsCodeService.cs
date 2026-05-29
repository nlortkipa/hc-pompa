using AutoMapper;
using Dal.Interfaces;
using Dal.Repositories;
using Data.Entities;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Services
{
    public class SmsCodeService : ISmsCodeService
    {
        private readonly IMapper _mapper;
        private readonly ISmsCodeRepository _smsCodeRepository;

        public SmsCodeService(IMapper mapper, ISmsCodeRepository smsCodeRepository)
        {
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
