using BussinessObject;
using DataTransferObject.DTO;
using Net.payOS.Types;
using Net.payOS;
using Repository.Interface.IUnitOfWork;
using Service.InterfaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class PaymentMethodService:IPaymentMethodService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentMethodService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PaymentMethod>> GetAllPaymentMethodAsync()
        {
            try
            {
                return await _unitOfWork.PaymentMethodRepo.GetAllPaymentMethodAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetAllPaymentMethodAsync,Service: {ex.Message}");
            }
        }
    }
}
