using BussinessObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Repository.Interface;
using Repository.Interface.IUnitOfWork;
using Repository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Repository
{
    public class UnitOfWork : IUnitOfWork
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
            MedicalReportRepo = new MedicalReportRepo(context);
            PartnerTypeRepo = new PartnerTypeRepo(context);
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
        public IMedicalReportRepo MedicalReportRepo { get; }
        public IPartnerTypeRepo PartnerTypeRepo { get; }

        public Task CommitTransactionAsync(IDbContextTransaction commit)
        {
            return commit.CommitAsync();

        }

        public Task RollBackAsync(IDbContextTransaction commit, string name)
        {
            return commit.RollbackToSavepointAsync(name);
        }

        public Task<int> SaveChangesAsync()
        {
            return _Context.SaveChangesAsync();
        }

        public Task StartTransactionAsync(string name)
        {
            var commit = _Context.Database.BeginTransaction();
            return commit.CreateSavepointAsync(name);
        }
    }
}
