using AutoMapper;
using Data.Entities;
using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<CreateUserDTO, User>();
            CreateMap<UpdateUserDTO, User>();

            // SmsCode
            CreateMap<SmsCode, SmsCodeDTO>().ReverseMap();

            // Company
            CreateMap<Company, CompanyDTO>().ReverseMap();
            CreateMap<CreateCompanyDTO, Company>();
            CreateMap<UpdateCompanyDTO, Company>();

            // Address
            CreateMap<Address, AddressDTO>().ReverseMap();
            CreateMap<CreateAddressDTO, Address>();
            CreateMap<UpdateAddressDTO, Address>();

            // Plate
            CreateMap<Plate, PlateDTO>();
            CreateMap<PlateDTO, Plate>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // PumpOperation
            CreateMap<PumpOperation, PumpOperationDto>()
                .ForMember(dest => dest.OperatorFirstName, opt => opt.MapFrom(src => src.Operator.FirstName))
                .ForMember(dest => dest.OperatorLastName, opt => opt.MapFrom(src => src.Operator.LastName))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(dest => dest.AddressName, opt => opt.MapFrom(src => src.Address.Name));
            CreateMap<PumpOperationDto, PumpOperation>();
            CreateMap<CreatePumpOperationDto, PumpOperation>();
        }
    }
}
