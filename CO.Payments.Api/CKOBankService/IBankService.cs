using CO.Payments.Api.Data.DTOs;

namespace CO.Payments.Api.Services;

public interface IBankService
{
    BankPaymentResponse MakePayment(BankPaymentRequest request);
}