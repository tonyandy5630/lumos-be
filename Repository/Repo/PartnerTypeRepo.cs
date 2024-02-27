using BussinessObject;
using DataAccessLayer;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository.Repo
{
    public class PartnerTypeRepo : IPartnerTypeRepo
    {
        public PartnerTypeRepo(LumosDBContext context) { }

        public Task<PartnerType?> GetPartnerTypeByIdAsync(int id) => PartnerTypeDAO.Instance.GetPartnertypeByIdAsync(id);

        public Task<List<PartnerType>> GetPartnerTypesAsync(string? keyword) => PartnerTypeDAO.Instance.GetPartnerTypesAsync(keyword);
    }
}
