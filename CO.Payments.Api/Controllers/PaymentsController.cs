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

    /// <summary>
    /// Takes a payment token, amount and currency to make an payment to the aquiring bank
    /// </summary>
    /// <param name="makePaymentRequest"></param>
    /// <param name="merchantId"></param>
    /// <returns>The payment details</returns>
    /// <remarks>
    /// Sample request:
    ///     POST /api/Payments
    ///     {
    ///         "token": "token",
    ///         "amount": 100,
    ///         "currecny": "GBP"
    ///     }
    ///     
    /// </remarks>
    // POST: api/Payments
    [HttpPost]
    public async Task<ActionResult<Payment>> MakePayment(MakePaymentRequest makePaymentRequest, [FromHeader] long merchantId)
    {
        var newPayment = await _paymentService
            .MakePaymentToAquiringBank(makePaymentRequest, merchantId);

        if (newPayment.PaymentStatus == Data.Lookups.PaymentStatus.Success)
        {
            return CreatedAtAction(
                "GetPayment",
                new { id = newPayment.PaymentReference },
                new GetPaymentResponse(newPayment)); 
        } else
        {
            return UnprocessableEntity();
        }
    }

    private bool PaymentExists(string id)
    {
        return (_context.Payments?.Any(e => e.PaymentReference == id)).GetValueOrDefault();
    }
}
