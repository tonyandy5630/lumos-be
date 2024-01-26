using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Interface;
using Repository.Interface.IUnitOfWork;
using Repository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DI
{
    public static class DependencInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string connectionString)
        {
            services.AddHttpContextAccessor();
            services.AddDbContext<LumosDBContext>(option =>
            option.UseSqlServer(connectionString));

            #region entity
            //unitofwork
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            //add repo
            services.AddTransient<IAdminRepo, AdminRepo>();

            services.AddTransient<IAddressRepo, AddressRepo>();

            services.AddTransient<IBookingLogRepo, BookingLogRepo>();

            services.AddTransient<IBookingRepo, BookingRepo>();

            services.AddTransient<ICustomerRepo, CustomerRepo>();

            services.AddTransient<IHistoryLogRepo, HistoryLogRepo>();

            services.AddTransient<IMedicalReportRepo, MedicalReportRepo>();

            services.AddTransient<IPartnerRepo, PartnerRepo>();

            services.AddTransient<IPartnerTypeRepo, PartnerTypeRepo>();

            services.AddTransient<IPaymentMethodRepo, PaymentMethodRepo>();

            services.AddTransient<IScheduleRepo, ScheduleRepo>();

            services.AddTransient<IServiceBookingRepo, ServiceBookingRepo>();

            services.AddTransient<IServiceCategoryRepo, ServiceCategoryRepo>();

            services.AddTransient<ISystemConfigurationRepo, SystemConfigurationRepo>();

            #endregion
            return services;
        }
    }
}
