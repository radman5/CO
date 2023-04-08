using Microsoft.AspNetCore.Mvc;
using CO.Payments.Api.Data.DbModels;
using CO.Payments.Api.Data.Database;
using CO.Payments.Api.Data.DTOs;
using CO.Payments.Api.TokenService;
using Microsoft.EntityFrameworkCore;

namespace CO.Payments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTokenController : ControllerBase
    {
        private readonly PaymentsDbContext _context;
        private readonly ITokenGenerator _tokenGenerator;
        private const string missingMerchantHeaderMessage = "Missing or invalid header 'MerchantId";

        public PaymentTokenController(PaymentsDbContext context, ITokenGenerator tokenGenerator)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
        }

        // GET: api/paymenttoken
        [HttpPost]
        public async Task<ActionResult<CreatePaymentTokenResponse>> CreatePaymentToken(CreatePaymentTokenRequest request)
        {
            var merchant = await GetMerchant();

            if (merchant != null)
            {
                var cardDetails = CreateNewCardDetailsFromCreateRequest(request, merchant);
                _context.CardDetails.Add(cardDetails);
                await _context.SaveChangesAsync();

                return new CreatePaymentTokenResponse
                {
                    Token = cardDetails.Token,
                };
            }

            return BadRequest(missingMerchantHeaderMessage);
        }

        // DELETE: api/paymenttoken/123abc
        [HttpDelete("{token}")]
        public async Task<IActionResult> DeleteCardDetails(string token)
        {
            var merchant = await GetMerchant();

            if (merchant != null)
            {
                var cardDetails = await _context.CardDetails.SingleOrDefaultAsync(cardDetails => 
                    cardDetails.Token == token && 
                    cardDetails.MerchantId == merchant.MerchantId);

                if (cardDetails == null)
                {
                    return NotFound();
                }

                _context.CardDetails.Remove(cardDetails);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            return BadRequest(missingMerchantHeaderMessage);
        }

        private CardDetails CreateNewCardDetailsFromCreateRequest(CreatePaymentTokenRequest request, MerchantPaymentProfile merchant)
        {
            var generatedToken = _tokenGenerator.GenerateToken(request);
            return new CardDetails
            {
                Token = generatedToken,
                CardNumber = request.CardNumber,
                CardHolder = request.CardHolder,
                Expiry = request.Expiry,
                Cvv = request.Cvv,
                MerchantId = merchant.MerchantId
            };
        }

        private async Task<MerchantPaymentProfile?> GetMerchant()
        {
            var merchantIdAsString = Request.Headers.SingleOrDefault(h => h.Key == "MerchantId").Value;
            if (long.TryParse(merchantIdAsString, out var merchantId))
            {
                var merchant = await _context.Merchants.FindAsync(merchantId);
                if (merchant != null)
                {
                    return merchant;
                }
            }

            return null;
        }
    }
}
