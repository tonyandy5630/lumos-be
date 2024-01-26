using DataAccessLayer.DBContext;
using Repository.Interface;
using Repository.Interface.IUnitOfWork;
using Repository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly LumosDBContext _Context;
        public UnitOfWork(
           LumosDBContext context
            )
        {
            _Context = context;
            AddressRepo = new AddressRepo(context);
            AdminRepo = new AdminRepo(context);
            BookingRepo = new BookingRepo(context);
            BookingLogRepo = new BookingLogRepo(context);
            CustomerRepo = new CustomerRepo(context);
            HistoryLogRepo = new HistoryLogRepo(context);
            PartnerRepo = new PartnerRepo(context);
            PaymentMethodRepo = new PaymentMethodRepo(context);
            ScheduleRepo = new ScheduleRepo(context);
            ServiceBookingRepo = new ServiceBookingRepo(context);
            ServiceCategoryRepo = new ServiceCategoryRepo(context);
            SystemConfigurationRepo = new SystemConfigurationRepo(context);
        }
        public LumosDBContext Context { get { return _Context; } }
        public IAddressRepo AddressRepo { get; }
        public IAdminRepo AdminRepo { get; }
        public IBookingRepo BookingRepo { get; }
        public IBookingLogRepo BookingLogRepo { get; }
        public ICustomerRepo CustomerRepo { get; }
        public IHistoryLogRepo HistoryLogRepo { get; }
        public IPartnerRepo PartnerRepo { get; }
        public IPaymentMethodRepo PaymentMethodRepo { get; }
        public IScheduleRepo ScheduleRepo { get; }
        public IServiceBookingRepo ServiceBookingRepo { get; }
        public IServiceCategoryRepo ServiceCategoryRepo { get; }
        public ISystemConfigurationRepo SystemConfigurationRepo { get; }
    }
}
