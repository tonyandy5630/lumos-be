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
                List<ItemData> items = new List<ItemData>();

                foreach (var itemRequest in request.Items)
                {
                    ItemData item = new ItemData(itemRequest.Name, itemRequest.Quantity, itemRequest.Amount);
                    items.Add(item);
                }
                long currentTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                long expireTimeStamp = currentTimeStamp + (1 * 60 * 60);
                PaymentData paymentData = new PaymentData(
                    request.OrderId,
                    request.TotalAmount,
                    request.Description,
                    items,
                    request.CancelUrl,
                    request.ReturnUrl,
                    request.Signature,
                    request.BuyerName,
                    request.BuyerEmail,
                    request.BuyerPhone,
                    request.BuyerAddress,
                    request.ExpiredAt = (int)expireTimeStamp
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
