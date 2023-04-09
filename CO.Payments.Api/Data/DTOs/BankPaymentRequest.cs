using CO.Payments.Api.Data.DbModels;

namespace CO.Payments.Api.Data.DTOs;

public class BankPaymentRequest
{
    public string MerchantBankReference { get; set; }
    public string CardNumber { get; set; }
    public string CardHolder { get; set; }
    public string Expiry { get; set; }
    public string Cvv { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }

    internal static BankPaymentRequest Create(MakePaymentRequest makePaymentRequest, CardDetails cardDetails)
    {
        return new BankPaymentRequest
        {
            CardNumber = cardDetails.CardNumber,
            CardHolder = cardDetails.CardHolder,
            Expiry = cardDetails.Expiry,
            Cvv = cardDetails.Cvv,
            Amount = makePaymentRequest.Amount,
            Currency = makePaymentRequest.Currency
        };
    }
}
