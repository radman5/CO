using CO.Payments.Api.Data.DTOs;

namespace CO.Payments.Api.Services;

public interface IBankService
{
    Task<BankPaymentResponse> MakePayment(BankPaymentRequest request);
}