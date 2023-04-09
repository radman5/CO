using CO.Payments.Api.Controllers.ExceptionHandling;
using CO.Payments.Api.Data.Database;
using CO.Payments.Api.Data.DbModels;
using CO.Payments.Api.Data.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CO.Payments.Api.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PaymentsDbContext _db;
        private readonly IBankService _bankService;

        public PaymentService(PaymentsDbContext db, IBankService bankService)
        {
            _db = db;
            _bankService = bankService;
        }

        public async Task<Payment> MakePaymentToAquiringBank(MakePaymentRequest makePaymentRequest, long merchantId)
        {
            var merchant = await _db.Merchants.FindAsync(merchantId);

            if (merchant == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Merchant not found");
            }

            var cardDetails = await _db.CardDetails.FindAsync(makePaymentRequest.Token);

            if (cardDetails == null ||
                cardDetails.Status != Data.Lookups.CardDetailsStatus.Pending)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid payment token");
            }

            cardDetails.Use(); // CardDetails are single use, make sure to mark them as used

            var newPayment = Payment.Create(makePaymentRequest, merchant.MerchantId, cardDetails);

            await _db.SaveChangesAsync();

            var bankPaymentResponse = _bankService.MakePayment(BankPaymentRequest.Create(makePaymentRequest, cardDetails));

            newPayment.SetToProcessed(bankPaymentResponse);

            await _db.SaveChangesAsync();

            return newPayment;
        }
    }

    public interface IPaymentService
    {
        Task<Payment> MakePaymentToAquiringBank(MakePaymentRequest makePaymentRequest, long merchantId);
    }
}
