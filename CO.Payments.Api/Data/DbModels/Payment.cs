using System.ComponentModel.DataAnnotations;
using CO.Payments.Api.Data.DTOs;
using CO.Payments.Api.Data.Lookups;

namespace CO.Payments.Api.Data.DbModels;

public class Payment
{
    [Key]
    public string PaymentReference { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public string StatusReason { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public string EndOfCardNumber { get; private set; }
    public string CardExpiry { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public long MerchantId { get; private set; }

    internal static Payment Create(MakePaymentRequest payment, long merchantId, CardDetails cardDetails)
    {
        return new Payment
        {
            PaymentReference = Guid.NewGuid().ToString(),
            PaymentStatus = PaymentStatus.Pending,
            Amount = payment.Amount,
            Currency = payment.Currency,
            EndOfCardNumber = cardDetails.CardNumber.Substring(cardDetails.CardNumber.Length - 5, 4),
            CardExpiry = cardDetails.Expiry,
            CreatedAt = DateTime.UtcNow,
            MerchantId = merchantId,
        };
    }

    internal void SetToProcessed(BankPaymentResponse bankPaymentResponse)
    {
        PaymentStatus = bankPaymentResponse.ResultType == 
            PaymentResult.Approved ? 
                PaymentStatus.Success : 
                PaymentStatus.Failed;
        StatusReason = bankPaymentResponse.Message;
        ProcessedAt = DateTime.UtcNow;
    }
}
