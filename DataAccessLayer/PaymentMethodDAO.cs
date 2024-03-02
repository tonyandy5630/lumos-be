using BussinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class PaymentMethodDAO
    {
        private static PaymentMethodDAO instance = null;
        private readonly LumosDBContext dbContext;

        public PaymentMethodDAO(LumosDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public static PaymentMethodDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PaymentMethodDAO(new LumosDBContext());
                }
                return instance;
            }
        }
        public async Task<List<PaymentMethod>> GetAllPaymentMethodAsync()
        {
            try
            {
                List<PaymentMethod> payments = await dbContext.PaymentMethods.ToListAsync();
                return payments;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAllPaymentMethodAsync", ex);
            }
        }
    }
}
