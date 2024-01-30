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
    public class PartnerRepo : IPartnerRepo
    {
        public PartnerRepo(LumosDBContext context) { }

        public Task<bool> AddPartnerAsync(Partner partner) =>PartnerDAO.Instance.AddPartnereAsync(partner);

        public Task<bool> BanPartnerAsync(int partnerId) => PartnerDAO.Instance.BanPartnerAsync(partnerId);

        public Task<List<Partner>> GetAllPartnersAsync() => PartnerDAO.Instance.GetAllPartnersAsync();

        public Task<Partner> GetPartnerByCodeAsync(string code) => PartnerDAO.Instance.GetPartnerByCodeAsync(code);

        public Task<Partner> GetPartnerByEmailAsync(string email) => PartnerDAO.Instance.GetPartnerByEmailAsync(email);

        public Task<Partner> GetPartnerByIDAsync(int id) => PartnerDAO.Instance.GetPartnerByIDAsync(id);

        public Task<Partner> GetPartnerByRefreshTokenAsync(string token) => PartnerDAO.Instance.GetPartnerByRefreshTokenAsync(token);

        public Task<PartnerService?> GetPartnerServiceDetailByIdAsync(int id) => PartnerDAO.Instance.GetPartnerServiceByIdAsync(id);

        public Task<IEnumerable<Partner>> SearchPartnerByPartnerOrServiceNameAsync(string keyword) => PartnerDAO.Instance.SearchPartnerByServiceOrPartnerNameAsync(keyword);

        public Task<bool> UpdatePartnerAsync(Partner partner) => PartnerDAO.Instance.UpdatePartnerAsync(partner);
    }
}
