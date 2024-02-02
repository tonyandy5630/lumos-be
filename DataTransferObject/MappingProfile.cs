using AutoMapper;
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

            CreateMap<AddPartnerServiceResquest, PartnerService>()
                .ForMember(dto => dto.Name, act => act.MapFrom(src => src.Name))
                .ForMember(dto => dto.Code, act => act.MapFrom(src => src.Code))
                .ForMember(dto => dto.Duration, act => act.MapFrom(src => src.Duration))
                .ForMember(dto => dto.Description, act => act.MapFrom(src => src.Description))
                .ForMember(dto => dto.Price, act => act.MapFrom(src => src.Price));
            
                
                
        }
    }
}