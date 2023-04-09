using CO.Payments.Api.Data.DbModels;
using Newtonsoft.Json;

namespace CO.Payments.Api.Data.DTOs;

public class GetPaymentResponse
{
    public string PaymentReference { get; set; }

    public string Status { get; set; }

    public string StatusReason { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; }
    public string EndOfCardNumber { get; set; }
    public string CardExpiry { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }

    public GetPaymentResponse() { }

    public GetPaymentResponse(Payment payment)
    {
        PaymentReference = payment.PaymentReference;
        Status = Enum.GetName(payment.PaymentStatus);
        StatusReason = payment.StatusReason;
        Amount = payment.Amount;
        Currency = payment.Currency;
        EndOfCardNumber = payment.EndOfCardNumber;
        CardExpiry = payment.CardExpiry;
        CreatedAt = payment.CreatedAt;
        ProcessedAt = payment.ProcessedAt;
    }
}