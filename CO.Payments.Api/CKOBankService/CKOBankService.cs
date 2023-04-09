using CO.Payments.Api.Data.DTOs;
using CO.Payments.Api.Data.Lookups;

namespace CO.Payments.Api.Services;

public class CKOBankService : IBankService
{
    private const string _insufficientFundsCardNumber = "insuffientfunds";

    public BankPaymentResponse MakePayment(BankPaymentRequest request)
    {
        if (request.CardNumber == _insufficientFundsCardNumber)
        {
            return new BankPaymentResponse
            {
                ResultType = PaymentResult.InsufficientFunds,
                Message = "Insufficient funds"
            };
        }

        return new BankPaymentResponse
        {
            ResultType = PaymentResult.Approved,
            Message = "Payment approved"
        };
    }
}
