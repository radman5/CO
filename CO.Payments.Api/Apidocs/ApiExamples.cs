using CO.Payments.Api.Data.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace CO.Payments.Api.Apidocs
{
    public class CreatePaymentTokenRequestExamples : IExamplesProvider<CreatePaymentTokenRequest>
    {
        CreatePaymentTokenRequest IExamplesProvider<CreatePaymentTokenRequest>.GetExamples()
        {
            return new CreatePaymentTokenRequest
            {
                CardNumber = "123456789123",
                CardHolder = "mr bean",
                Expiry = "10/30",
                Cvv = "123"
            };
        }
    }

    public class MakePaymentRequestExamples : IExamplesProvider<MakePaymentRequest>
    {
        public MakePaymentRequest GetExamples()
        {
            return new MakePaymentRequest
            {
                Token = "token",
                Amount = 100,
                Currency = "GBP"
            };
        }
    }
}