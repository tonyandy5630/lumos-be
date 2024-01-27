using BussinessObject;
using Repository.GenericRepository;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class PartnerRepo:IPartnerRepo
    {
        public PartnerRepo(LumosDBContext context) { }

        public Task<PartnerService> GetPartnerServiceDetailByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
