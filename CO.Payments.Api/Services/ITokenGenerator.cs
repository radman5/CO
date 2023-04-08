using CO.Payments.Api.Data.DTOs;

namespace CO.Payments.Api.TokenService;

public interface ITokenGenerator
{
    string GenerateToken(CreatePaymentTokenRequest request);
}