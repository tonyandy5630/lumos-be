using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.IUnitOfWork
{
     public interface IUnitOfWork
    {

        IAddressRepo AddressRepo { get; }
        IAdminRepo AdminRepo { get; }
        IBookingRepo BookingRepo { get; }
        IBookingLogRepo BookingLogRepo { get; }
        ICustomerRepo CustomerRepo { get; }
        IHistoryLogRepo HistoryLogRepo { get; }
        IPartnerRepo PartnerRepo { get; }
        IPaymentMethodRepo PaymentMethodRepo { get; }
        IScheduleRepo ScheduleRepo { get; }
        IServiceBookingRepo ServiceBookingRepo { get; }
        IServiceCategoryRepo ServiceCategoryRepo { get; }
        ISystemConfigurationRepo SystemConfigurationRepo { get; }

        Task<int> SaveChangesAsync();
        Task StartTransactionAsync(string name);
        Task CommitTransactionAsync(IDbContextTransaction commit);
        Task RollBackAsync(IDbContextTransaction commit, string name);


    }
}
