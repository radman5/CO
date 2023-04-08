namespace CO.Payments.Api.Data.DTOs
{
    public class CreatePaymentTokenRequest
    {
        public string CardNumber { get; set; }
        public string CardHolder { get; set; }
        public string Expiry { get; set; }
        public string Cvv { get; set; }
    }
}
