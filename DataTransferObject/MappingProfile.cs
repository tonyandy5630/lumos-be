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
            CreateMap<Partner, PartnerDTO>()
               .ForMember(dto => dto.PartnerId, opt => opt.MapFrom(src => src.PartnerId))
               .ForMember(dto => dto.TypeId, opt => opt.MapFrom(src => src.TypeId))
               .ForMember(dto => dto.Code, opt => opt.MapFrom(src => src.Code))
               .ForMember(dto => dto.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dto => dto.PartnerName, opt => opt.MapFrom(src => src.PartnerName))
               .ForMember(dto => dto.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
               .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
               .ForMember(dto => dto.Address, opt => opt.MapFrom(src => src.Address))
               .ForMember(dto => dto.Description, opt => opt.MapFrom(src => src.Description))
               .ForMember(dto => dto.IsDelete, opt => opt.MapFrom(src => src.Status != 1)) // Ánh xạ ngược từ Status
               .ForMember(dto => dto.LastLogin, opt => opt.MapFrom(src => src.LastLogin))
               .ForMember(dto => dto.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dto => dto.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
               .ForMember(dto => dto.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
               .ForMember(dto => dto.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
               .ForMember(dto => dto.ImgUrl, opt => opt.MapFrom(src => src.ImgUrl))
               .ForMember(dto => dto.RefreshToken, opt => opt.MapFrom(src => src.RefreshToken))
               .ForMember(dto => dto.BusinessLicenseNumber, opt => opt.MapFrom(src => src.BusinessLicenseNumber))
               .ForMember(dto => dto.Role, opt => opt.MapFrom(src => src.Role))
               .ForMember(dto => dto.Rating, opt => opt.MapFrom(src => src.Rating));

            CreateMap<PartnerDTO, Partner>()
                .ForMember(dest => dest.PartnerId, opt => opt.MapFrom(src => src.PartnerId))
                .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.TypeId))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PartnerName, opt => opt.MapFrom(src => src.PartnerName))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsDelete ? 0 : 1)) // Ánh xạ ngược từ IsDelete
                .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLogin))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
                .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.ImgUrl))
                .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.RefreshToken))
                .ForMember(dest => dest.BusinessLicenseNumber, opt => opt.MapFrom(src => src.BusinessLicenseNumber))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));

            CreateMap<AddAddressRequest, Address>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.displayName))
                .ForMember(dest => dest.Address1, opt => opt.MapFrom(src => src.address1))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.customerId));

            CreateMap<CreateBookingDTO, Booking>()
            .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.PaymentId))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
            .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
/*            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))*/
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            CreateMap<RegistrationModel, Customer>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dto => dto.Fullname, opt => opt.MapFrom(src => src.Fullname))
                .ForMember(dto => dto.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));

            CreateMap<AddCustomerRequest, Customer>()
                .ForMember(dto => dto.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dto => dto.Fullname, opt => opt.MapFrom(src => src.Fullname))
                .ForMember(dto => dto.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dto => dto.ImgUrl, opt => opt.MapFrom(src => src.ImgUrl));

            CreateMap<PartnerService, PartnerServiceDTO>()
                .ForMember(dto => dto.ServiceId, act => act.MapFrom(src => src.ServiceId))
                .ForMember(dto => dto.Name, act => act.MapFrom(src => src.Name))
                .ForMember(dto => dto.Code, act => act.MapFrom(src => src.Code))
                .ForMember(dto => dto.Duration, act => act.MapFrom(src => src.Duration))
                .ForMember(dto => dto.Status, act => act.MapFrom(src => src.Status))
                .ForMember(dto => dto.Description, act => act.MapFrom(src => src.Description))
                .ForMember(dto => dto.Price, act => act.MapFrom(src => src.Price))
                .ForMember(dto => dto.Rating, act => act.MapFrom(src => src.Rating));

            CreateMap<PartnerService, BillServiceDTO>()
                .ForMember(dto => dto.Name, act => act.MapFrom(src => src.Name))
                .ForMember(dto => dto.Price, act => act.MapFrom(src => src.Price));


            CreateMap<ServiceCategory, ServiceCategoryDTO>()
                .ForMember(dto => dto.CategoryId, act => act.MapFrom(src => src.CategoryId))
                .ForMember(dto => dto.Category, act => act.MapFrom(src => src.Category))
                .ForMember(dto => dto.Code, act => act.MapFrom(src => src.Code));

            CreateMap<Partner, SearchPartnerDTO>()
                .ForMember(dto => dto.PartnerId, act => act.MapFrom(src => src.PartnerId))
                .ForMember(dto => dto.Role, act => act.MapFrom(src => src.Role))
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
                .ForMember(dto => dto.Fullname, act => act.MapFrom(src => src.Fullname))
                .ForMember(dto => dto.Phone, act => act.MapFrom(src => src.Phone))
                .ForMember(dto => dto.Dob, act => act.MapFrom(src => src.Dob))
                .ForMember(dto => dto.Gender, act => act.MapFrom(src => src.Gender))
                .ForMember(dto => dto.Pronounce, act => act.MapFrom(src => src.Pronounce))
                .ForMember(dto => dto.BloodType, act => act.MapFrom(src => src.BloodType))
                .ForMember(dto => dto.Note, act => act.MapFrom(src => src.Note));

            CreateMap<PartnerRequest, Partner>()
                .ForMember(req => req.PartnerName, act => act.MapFrom(src => src.PartnerName))
                .ForMember(req => req.BusinessLicenseNumber, act => act.MapFrom(src => src.BusinessLicenseNumber))
                .ForMember(req => req.DisplayName, act => act.MapFrom(src => src.DisplayName))
                .ForMember(req => req.Address, act => act.MapFrom(src => src.Address))
                .ForMember(req => req.Email, act => act.MapFrom(src => src.Email))
                .ForMember(req => req.Phone, act => act.MapFrom(src => src.Phone))
                .ForMember(req => req.TypeId, act => act.MapFrom(src => src.TypeId));

            CreateMap<AddPartnerServiceResquest, PartnerService>()
                .ForMember(dto => dto.Name, act => act.MapFrom(src => src.Name))
                .ForMember(dto => dto.Duration, act => act.MapFrom(src => src.Duration))
                .ForMember(dto => dto.Description, act => act.MapFrom(src => src.Description))
                .ForMember(dto => dto.Price, act => act.MapFrom(src => src.Price));

            CreateMap<UpdatePartnerServiceRequest, PartnerService>()
                .ForMember(dto => dto.Name, act => act.MapFrom(src => src.Name))
                .ForMember(dto => dto.Duration, act => act.MapFrom(src => src.Duration))
                .ForMember(dto => dto.Description, act => act.MapFrom(src => src.Description))
                .ForMember(dto => dto.Price, act => act.MapFrom(src => src.Price));

            CreateMap<ServiceBooking, ServiceDTO>()
                .ForMember(dto => dto.ServiceId, opt => opt.MapFrom(src => src.ServiceId));

            CreateMap<ScheduleRequest, Schedule>()
                .ForMember(req => req.WorkShift, act => act.MapFrom(src => src.WorkShift))
                .ForMember(req => req.DayOfWeek, act => act.MapFrom(src => src.DayOfWeek))
                .ForMember(req => req.Note, act => act.MapFrom(src => src.Note));
                

        }
    }
}