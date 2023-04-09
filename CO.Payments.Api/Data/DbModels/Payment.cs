using System.ComponentModel.DataAnnotations;
using CO.Payments.Api.Data.DTOs;
using CO.Payments.Api.Data.Lookups;

namespace CO.Payments.Api.Data.DbModels;

public class Payment
{
    [Key]
    public string PaymentReference { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string StatusReason { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string EndOfCardNumber { get; set; }
    public string CardExpiry { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public long MerchantId { get; set; }

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
}
