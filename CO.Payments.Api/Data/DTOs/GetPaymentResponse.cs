using CO.Payments.Api.Data.DbModels;

namespace CO.Payments.Api.Data.DTOs;

public class GetPaymentResponse
{
    public string PaymentReference { get; private set; }
    public string Status { get; private set; }
    public string StatusReason { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public string EndOfCardNumber { get; private set; }
    public string CardExpiry { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

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