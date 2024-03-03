using RequestEntity;
using Service.InterfaceService;
using Net.payOS;
using Net.payOS.Types;
using static RequestEntity.Constraint.Constraint;
using DataTransferObject.DTO;
using Repository.Interface.IUnitOfWork;
using Repository;

namespace Service.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly PayOS payOS;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentService(IUnitOfWork unitOfWork)
        {
            string clientId = PaymentsConstraint.clientId;
            string apiKey = PaymentsConstraint.apiKey;
            string checksumKey = PaymentsConstraint.checksumKey;
            payOS = new PayOS(clientId, apiKey, checksumKey);
            _unitOfWork = unitOfWork;
        }

        public async Task<PaymentResponse> CreatePaymentLink(PaymentRequest request)
        {
            try
            {
                List<ItemData> items = new List<ItemData>();
                List<(string ServiceName, int? Price, int Quantity)> bookingServiceInfo = await _unitOfWork.BookingRepo.GetBookingServiceInfoAsync(request.BookingId);

                Dictionary<(string ServiceName, int? Price), int> serviceQuantities = new Dictionary<(string ServiceName, int? Price), int>();

                foreach (var itemRequest in bookingServiceInfo)
                {
                    var key = (itemRequest.ServiceName, itemRequest.Price);
                    if (serviceQuantities.ContainsKey(key))
                    {
                        // Nếu đã tồn tại ServiceName và Price trong Dictionary, tăng Quantity lên
                        serviceQuantities[key] += itemRequest.Quantity;
                    }
                    else
                    {
                        // Nếu chưa tồn tại, thêm ServiceName và Price vào Dictionary
                        serviceQuantities.Add(key, itemRequest.Quantity);
                    }
                }

                foreach (var kvp in serviceQuantities)
                {
                    ItemData item = new ItemData(kvp.Key.ServiceName, kvp.Value, kvp.Key.Price ?? 0);
                    items.Add(item);
                }

                int? totalPrice = await _unitOfWork.BookingRepo.GetTotalPriceByBookingIdAsync(request.BookingId);


                long currentTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                long expireTimeStamp = currentTimeStamp + (20 * 60); // 20 minutes
                PaymentData paymentData = new PaymentData(
                    request.BookingId,
                    (int)totalPrice,
                    request.Description,
                    items,
                    "",
                    "",
                    "",
                    request.BuyerName,
                    request.BuyerEmail,
                    request.BuyerPhone,
                    request.BuyerAddress,
                    (int)expireTimeStamp
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
