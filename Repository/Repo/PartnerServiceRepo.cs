using BussinessObject;
using DataAccessLayer;
using DataTransferObject.DTO;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class PartnerServiceRepo : IPartnerServiceRepo
    {
        public PartnerServiceRepo(LumosDBContext context) { }

        public Task<bool> DeletePartnerServiceAsync(int id) => PartnerServiceDAO.Instance.DeletePartnerServiceAsync(id);

        public Task<PartnerService?> GetPartnerServiceByIdAsync(int serviceId) => PartnerServiceDAO.Instance.GetPartnerServiceByIdAsync(serviceId);

        public Task<PartnerService?> GetPartnerServiceByServiceNameAsync(string serviceName, int partnerId) => PartnerServiceDAO.Instance.GetServiceOfPartnerByServiceNameAsync(serviceName, partnerId);

        public Task<List<PartnerServiceDTO>> GetServiceBookedByMedicalReportIdAndBookingId(int medicalReportId, int BookingId) => PartnerServiceDAO.Instance.GetServiceBookedByMedicalReportIdAndBookingId(medicalReportId, BookingId);

        public Task<IEnumerable<PartnerServiceDTO>> GetTopFiveBookedServicesAsync() => PartnerServiceDAO.Instance.GetTopFiveBookedServicesAsync();

        public Task<bool> UpdatePartnerServiceAsync(PartnerService service) => PartnerServiceDAO.Instance.UpdatePartnerServiceAsync(service);
    }
}
