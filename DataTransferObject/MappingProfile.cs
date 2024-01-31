﻿using AutoMapper;
using BussinessObject;
using BussinessObject.AuthenModel;
using DataTransferObject.DTO;

namespace DataTransferObject
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegistrationModel, Customer>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));

            CreateMap<PartnerService, PartnerServiceDTO>()
                .ForMember(dto => dto.ServiceId, act => act.MapFrom(src => src.ServiceId))
                .ForMember(dto => dto.Name, act => act.MapFrom(src => src.Name))
                .ForMember(dto => dto.Code, act => act.MapFrom(src => src.Code))
                .ForMember(dto => dto.Duration, act => act.MapFrom(src => src.Duration))
                .ForMember(dto => dto.Status, act => act.MapFrom(src => src.Status))
                .ForMember(dto => dto.Description, act => act.MapFrom(src => src.Description))
                .ForMember(dto => dto.Price, act => act.MapFrom(src => src.Price));

            CreateMap<ServiceCategory, ServiceCategoryDTO>()
                .ForMember(dto => dto.CategoryId, act => act.MapFrom(src => src.CategoryId))
                .ForMember(dto => dto.Category, act => act.MapFrom(src => src.Category))
                .ForMember(dto => dto.Code, act => act.MapFrom(src => src.Code));
        }
    }
}