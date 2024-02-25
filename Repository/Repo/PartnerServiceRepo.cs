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


        public Task<PartnerService?> GetPartnerServiceByServiceNameAsync(string serviceName, int partnerId) => PartnerServiceDAO.Instance.GetServiceOfPartnerByServiceNameAsync(serviceName, partnerId);

        public Task<IEnumerable<PartnerServiceDTO>> GetTopFiveBookedServicesAsync() => PartnerServiceDAO.Instance.GetTopFiveBookedServicesAsync();
    }
}
