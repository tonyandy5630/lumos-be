using RequestEntity;
using Service.InterfaceService;
using Net.payOS;
using Net.payOS.Types;
using static RequestEntity.Constraint.Constraint;
using DataTransferObject.DTO;

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

        public async Task<PaymentResponse> CreatePaymentLink(PaymentRequest request)
        {
            try
            {
                ItemData item = new ItemData(request.Name, request.Quantity=1, request.Amount);
                List<ItemData> items = new List<ItemData> { item };

                PaymentData paymentData = new PaymentData(
                    request.OrderId,
                    request.TotalAmount,
                    request.Description,
                    items,
                    null,
                    null
                );
                CreatePaymentResult createPayment = await payOS.createPaymentLink(paymentData);

                return new PaymentResponse
                {
                    Bin = createPayment.bin,
                    AccountNumber = createPayment.accountNumber,
                    Amount = createPayment.amount,
                    Description = createPayment.description,
                    OrderCode = createPayment.orderCode,
                    Currency = createPayment.currency,
                    PaymentLinkId = createPayment.paymentLinkId,
                    Status = createPayment.status,
                    CheckoutUrl = createPayment.checkoutUrl,
                    QrCode = createPayment.qrCode,
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating payment link: {ex.Message}");
            }
        }
    }
}
