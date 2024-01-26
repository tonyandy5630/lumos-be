using BussinessObject;
using Repository.Interface;
using Repository.Interface.IUnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class AddressRepo:IAddressRepo
    {
        public AddressRepo(LumosDBContext context)
        {
        }
    }
}
