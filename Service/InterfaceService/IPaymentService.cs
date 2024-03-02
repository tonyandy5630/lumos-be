using DataTransferObject.DTO;
using RequestEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.InterfaceService
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentLink(PaymentRequest request);
    }
}
