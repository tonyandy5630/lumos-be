using BussinessObject;
using DataAccessLayer;
using DataTransferObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Repository.Interface;
using Repository.Interface.IUnitOfWork;
using Repository.Repo;
using Service.InterfaceService;
using Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DI
{
    public static class DependencInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string connectionString)
        {
            services.AddHttpContextAccessor();
            services.AddDbContext<LumosDBContext>(option =>
            option.UseSqlServer(connectionString), ServiceLifetime.Transient);


            #region entity
            //authentication
            services.AddTransient<IAuthentication, Authentication>();
            //unitofwork
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            //add repo

            // Admin
            services.AddTransient<IAdminRepo, AdminRepo>();
            services.AddTransient<IAdminService, AdminService>();

            // Address
            services.AddTransient<IAddressRepo, AddressRepo>();
            services.AddTransient<IAddressService, AddressService>();

            //BookingLog
            services.AddTransient<IBookingLogRepo, BookingLogRepo>();
            services.AddTransient<IBookingLogService, BookingLogService>();

            services.AddTransient<IBookingRepo, BookingRepo>();
            services.AddTransient<IBookingService, BookingService>();


            services.AddTransient<ICustomerRepo, CustomerRepo>();
            services.AddTransient<ICustomerService, CustomerService>();

            services.AddTransient<IHistoryLogRepo, HistoryLogRepo>();
            services.AddTransient<IHistoryLogService, HistoryLogService>();

            services.AddTransient<IMedicalReportRepo, MedicalReportRepo>();
            services.AddTransient<IMedicalReportService, MedicalReportService>();

            //ServiceDetail
            services.AddTransient<IServiceDetailRepo, ServiceDetailRepo>();

            // Partner
            services.AddTransient<IPartnerRepo, PartnerRepo>();
            services.AddTransient<IPartnerService, PartnerServices>();

            //ParterService
            services.AddTransient<IPartnerServiceRepo, PartnerServiceRepo>();

            services.AddTransient<IPartnerTypeRepo, PartnerTypeRepo>();
            services.AddTransient<IPartnerService, PartnerServices>();

            services.AddTransient<IPaymentMethodRepo, PaymentMethodRepo>();
            services.AddTransient<IPaymentMethodService, PaymentMethodService>();

            services.AddTransient<IScheduleRepo, ScheduleRepo>();
            services.AddTransient<IScheduleService, ScheduleService>();

            services.AddTransient<IServiceBookingRepo, ServiceBookingRepo>();
            services.AddTransient<IServiceBookingSer, ServiceBookingSer>();

            services.AddTransient<IServiceCategoryRepo, ServiceCategoryRepo>();
            services.AddTransient<IServiceCategorySer, ServiceCategorySer>();

            services.AddTransient<ISystemConfigurationRepo, SystemConfigurationRepo>();
            services.AddTransient<ISystemConfigurationService, SystemConfigurationService>();


            #endregion

            //AUTOMAPPER
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            return services;
        }
    }
}
