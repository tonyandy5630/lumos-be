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

            CreateMap<CustomerServiceDTO, Customer>()
                .ForMember(dto => dto.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dto => dto.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dto => dto.Fullname, opt => opt.MapFrom(src => src.Fullname))
                .ForMember(dto => dto.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dto => dto.Pronounce, opt => opt.MapFrom(src => src.Pronounce))
                .ForMember(dto => dto.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dto => dto.LastLogin, opt => opt.MapFrom(src => src.LastLogin))
                .ForMember(dto => dto.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(dto => dto.UpdateBy, opt => opt.MapFrom(src => src.UpdateBy))
                .ForMember(dto => dto.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dto => dto.ImgUrl, opt => opt.MapFrom(src => src.ImgUrl))
                .ForMember(dto => dto.RefreshToken, opt => opt.MapFrom(src => src.RefreshToken));

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

            CreateMap<Partner, SearchPartnerDTO>()
                .ForMember(dto => dto.PartnerId, act => act.MapFrom(src => src.PartnerId))
                .ForMember(dto => dto.Code, act => act.MapFrom(src => src.Code))
                .ForMember(dto => dto.Email, act => act.MapFrom(src => src.Email))
                .ForMember(dto => dto.PartnerName, act => act.MapFrom(src => src.PartnerName))
                .ForMember(dto => dto.Address, act => act.MapFrom(src => src.Address))
                .ForMember(dto => dto.Address, act => act.MapFrom(src => src.Address))
                .ForMember(dto => dto.LastLogin, act => act.MapFrom(src => src.LastLogin))
                .ForMember(dto => dto.ImgUrl, act => act.MapFrom(src => src.ImgUrl))
                .ForMember(dto => dto.ImgUrl, act => act.MapFrom(src => src.ImgUrl))
                .ForMember(dto => dto.Type, act => act.MapFrom(src => src.Type));
                
        }
    }
}