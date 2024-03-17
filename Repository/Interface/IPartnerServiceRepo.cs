using BussinessObject;
using DataTransferObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IPartnerServiceRepo
    {
        Task<PartnerService?> GetPartnerServiceByServiceNameAsync(string serviceName, int partnerId);
        Task<IEnumerable<PartnerServiceDTO>> GetTopFiveBookedServicesAsync();
        Task<List<PartnerServiceDTO>> GetServiceBookedByMedicalReportIdAndBookingId(int medicalReportId, int BookingId);
        Task<bool> DeletePartnerServiceAsync(int id);
        Task<bool> UpdatePartnerServiceAsync(PartnerService service);
        Task<PartnerService?> GetPartnerServiceByIdAsync(int serviceId);
    }
}
