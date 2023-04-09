using Microsoft.Identity.Client;

namespace CO.Payments.Api.Data.DTOs;

public class MakePaymentRequest
{
    public string Token { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
}