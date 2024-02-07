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
        private LumosDBContext _Context;
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
            ServiceDetailRepo = new ServiceDetailRepo(context);
            PartnerServiceRepo = new PartnerServiceRepo(context);
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
        public IPartnerServiceRepo PartnerServiceRepo { get; }
        public IServiceDetailRepo ServiceDetailRepo { get; }
        public IMedicalReportRepo MedicalReportRepo { get; }
        public IPartnerTypeRepo PartnerTypeRepo { get; }
        
        public Task AttachDbContext(LumosDBContext dbContext)
        {
            _Context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            return Task.CompletedTask;
        }


        public Task CommitTransactionAsync(IDbContextTransaction commit)
        {
            return commit.CommitAsync();

        }

        public Task DetachDbContext()
        {
            _Context = null;
            return Task.CompletedTask;
        }

        public Task RollBackAsync(IDbContextTransaction commit, string name)
        {
            return commit.RollbackToSavepointAsync(name);
        }

        public Task<int> SaveChangesAsync()
        {
            return _Context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> StartTransactionAsync(string name)
        {
            var commit = _Context.Database.BeginTransaction();
            await commit.CreateSavepointAsync(name);
            return commit;
        }
    }
}
