using CO.Payments.Api.Data.Lookups;

namespace CO.Payments.Api.Data.DTOs;

public class BankPaymentResponse
{
    public string PaymentId { get; set; }
    public PaymentResult ResultType { get; set; }
    public string Message { get; set; }
}
