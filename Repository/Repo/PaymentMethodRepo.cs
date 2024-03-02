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
    public class PaymentMethodRepo:IPaymentMethodRepo
    {
        public PaymentMethodRepo(LumosDBContext context) { }

        public Task<List<PaymentMethod>> GetAllPaymentMethodAsync() => PaymentMethodDAO.Instance.GetAllPaymentMethodAsync();
    }
}
