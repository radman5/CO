using Microsoft.AspNetCore.Mvc;
using CO.Payments.Api.Data.DbModels;
using CO.Payments.Api.Data.Database;
using CO.Payments.Api.Data.DTOs;
using Microsoft.EntityFrameworkCore;
using CO.Payments.Api.Controllers.ExceptionHandling;
using System.Net;

namespace CO.Payments.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentTokenController : MerchantControllerBase
{
    private readonly PaymentsDbContext _context;

    public PaymentTokenController(PaymentsDbContext context)
    {
        _context = context;
    }

    // POST: api/paymenttoken
    [HttpPost]
    public async Task<ActionResult<CreatePaymentTokenResponse>> CreatePaymentToken(CreatePaymentTokenRequest request, [FromHeader] long merchantId)
    {
        await ValidateMerchantExists(_context, Request, merchantId);

        var cardDetails = CardDetails.Create(request.CardNumber, request.CardHolder, request.Expiry, request.Cvv, merchantId);
        _context.CardDetails.Add(cardDetails);
        await _context.SaveChangesAsync();

        return new CreatePaymentTokenResponse
        {
            Token = cardDetails.Token,
        };
    }

    // DELETE: api/paymenttoken/123abc
    [HttpDelete("{token}")]
    public async Task<IActionResult> DeleteCardDetails(string token, [FromHeader] long merchantId)
    {
        await ValidateMerchantExists(_context, Request, merchantId);

        var cardDetails = await _context.CardDetails.SingleOrDefaultAsync(cardDetails =>
            cardDetails.Token == token &&
            cardDetails.MerchantId == merchantId);

        if (cardDetails == null)
        {
            return NotFound();
        }

        _context.CardDetails.Remove(cardDetails);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
