namespace CO.Payments.Api.Data.DTOs;

public class BankPaymentRequest
{
    public string MerchantId { get; set; }
    public string MerchantBankReference { get; set; }
    public string CardNumber { get; set; }
    public string CardHolder { get; set; }
    public string Expiry { get; set; }
    public string Cvv { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
}
