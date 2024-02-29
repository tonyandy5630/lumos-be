using RequestEntity;
using Service.InterfaceService;
using Net.payOS;
using Net.payOS.Types;
using static RequestEntity.Constraint.Constraint;

namespace Service.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly PayOS payOS;

        public PaymentService()
        {
            string clientId = PaymentsConstraint.clientId;
            string apiKey = PaymentsConstraint.apiKey;
            string checksumKey = PaymentsConstraint.checksumKey;
            payOS = new PayOS(clientId, apiKey, checksumKey);
        }

        public async Task<string> CreatePaymentLink(PaymentRequest request)
        {
            try
            {
                ItemData item = new ItemData(request.Name, request.Quantity, request.Amount);
                List<ItemData> items = new List<ItemData> { item };

                PaymentData paymentData = new PaymentData(request.OrderId, request.TotalAmount, request.Description,
                     items, request.SuccessUrl, request.CancelUrl);

                CreatePaymentResult createPayment = await payOS.createPaymentLink(paymentData);

                return createPayment.checkoutUrl;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating payment link: {ex.Message}");
            }
        }
    }
}
