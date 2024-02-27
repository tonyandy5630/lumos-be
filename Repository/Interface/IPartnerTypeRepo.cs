using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IPartnerTypeRepo
    {
        Task<List<PartnerType>> GetPartnerTypesAsync(string? keyword);

        Task<PartnerType?> GetPartnerTypeByIdAsync(int id);
    }
}
