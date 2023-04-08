using CO.Payments.Api.Data.DTOs;

namespace CO.Payments.Api.TokenService;

public class TokenGenerator : ITokenGenerator
{
    public string GenerateToken(CreatePaymentTokenRequest request)
    {
        return Guid.NewGuid().ToString();
    }
}