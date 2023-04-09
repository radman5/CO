using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CO.Payments.Api.Data.Database;
using CO.Payments.Api.Data.DbModels;
using CO.Payments.Api.Data.DTOs;
using System.Net;
using CO.Payments.Api.Controllers.ExceptionHandling;
using CO.Payments.Api.Services;

namespace CO.Payments.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : MerchantControllerBase
{
    private readonly PaymentsDbContext _context;
    private readonly IPaymentService _paymentService;

    public PaymentsController(PaymentsDbContext context, IPaymentService paymentService)
    {
        _context = context;
        _paymentService = paymentService;
    }

    // GET: api/Payments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetPaymentResponse>>> GetPayment([FromQuery] string last, [FromQuery] int pageSize)
    {
        // TODO: Enable filtering

        int maxPageSize = 50;
        int actualPageSize = Math.Clamp(pageSize, 1, maxPageSize);
        return await _context.Payments
            .OrderByDescending(x => x.CreatedAt)
            .Where(x => x.PaymentReference == last)
            .Skip(1)
            .Take(actualPageSize)
            .Select(x => new GetPaymentResponse(x)).ToListAsync();
    }

    // GET: api/Payments/abc123
    [HttpGet("{paymentReference}")]
    public async Task<ActionResult<GetPaymentResponse>> GetPayment(string paymentReference, [FromHeader] long merchantId)
    {
        await ValidateMerchantExists(_context, Request, merchantId);

        var payment = await _context.Payments.SingleOrDefaultAsync(x =>
            x.PaymentReference == paymentReference &&
            x.MerchantId == merchantId);

        if (payment == null)
        {
            return NotFound();
        }

        return new GetPaymentResponse(payment);
    }

    // POST: api/Payments
    [HttpPost]
    public async Task<ActionResult<Payment>> MakePayment(MakePaymentRequest makePaymentRequest, [FromHeader] long merchantId)
    {
        var newPayment = await _paymentService
            .MakePaymentToAquiringBank(makePaymentRequest, merchantId);

        return CreatedAtAction(
            "GetPayment",
            new { id = newPayment.PaymentReference },
            new GetPaymentResponse(newPayment));
    }

    private async Task<Payment> CreateNewPayment(MakePaymentRequest makePaymentRequest, long merchantId)
    {
        var merchant = await _context.Merchants.FindAsync(merchantId);

        if (merchant != null)
        {
            var cardDetails = await _context.CardDetails.FindAsync(makePaymentRequest.Token);
            if (cardDetails != null &&
                cardDetails.Status == Data.Lookups.CardDetailsStatus.Pending)
            {
                cardDetails.Use(); // TODO: Think of a better place to do this
                var newPayment = Payment.Create(makePaymentRequest, merchant.MerchantId, cardDetails);
                return newPayment;
            }

            throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid payment token");
        }

        throw new HttpResponseException(HttpStatusCode.BadRequest, "Merchant profile not found");
    }

    private bool PaymentExists(string id)
    {
        return (_context.Payments?.Any(e => e.PaymentReference == id)).GetValueOrDefault();
    }
}
