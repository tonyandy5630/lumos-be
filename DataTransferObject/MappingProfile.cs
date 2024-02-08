﻿using AutoMapper;
using BussinessObject;
using BussinessObject.AuthenModel;
using DataTransferObject.DTO;
using RequestEntity;

namespace DataTransferObject
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateBookingDTO, Booking>()
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.PaymentId))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => TimeSpan.Parse(src.From)))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

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

            CreateMap<MedicalReportDTO, MedicalReport>()
                /*                .ForMember(dto => dto.ReportId, act => act.MapFrom(src => src.ReportId))
                                .ForMember(dto => dto.Code, act => act.MapFrom(src => src.Code))*/
                .ForMember(dto => dto.Fullname, act => act.MapFrom(src => src.Fullname))
                .ForMember(dto => dto.Phone, act => act.MapFrom(src => src.Phone))
                .ForMember(dto => dto.Dob, act => act.MapFrom(src => src.Dob))
                .ForMember(dto => dto.Gender, act => act.MapFrom(src => src.Gender))
                .ForMember(dto => dto.Pronounce, act => act.MapFrom(src => src.Pronounce))
                .ForMember(dto => dto.BloodType, act => act.MapFrom(src => src.BloodType))
                .ForMember(dto => dto.Note, act => act.MapFrom(src => src.Note));
            /*                .ForMember(dto => dto.Status, act => act.MapFrom(src => src.Status));*/

            CreateMap<AddPartnerServiceResquest, PartnerService>()
                .ForMember(dto => dto.Name, act => act.MapFrom(src => src.Name))
                .ForMember(dto => dto.Code, act => act.MapFrom(src => src.Code))
                .ForMember(dto => dto.Duration, act => act.MapFrom(src => src.Duration))
                .ForMember(dto => dto.Description, act => act.MapFrom(src => src.Description))
                .ForMember(dto => dto.Price, act => act.MapFrom(src => src.Price));

            CreateMap<AddPartnerScheduleRequest, Schedule>()
                .ForMember(dto => dto.DayOfWeek, act => act.MapFrom(src => src.DayOfWeek))
                .ForMember(dto => dto.From, act => act.MapFrom(src => src.From))
                .ForMember(dto => dto.To, act => act.MapFrom(src => src.To))
                .ForMember(dto => dto.Note, act => act.MapFrom(src => src.Note));
        }
    }
}