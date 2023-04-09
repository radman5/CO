using CO.Payments.Api.Controllers.ExceptionHandling;
using CO.Payments.Api.Data.Database;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CO.Payments.Api.Controllers;

public abstract class MerchantControllerBase : ControllerBase
{
    protected static async Task ValidateMerchantExists(PaymentsDbContext _db, HttpRequest request, long merchantId)
    {
        var merchantIdAsString = request.Headers.SingleOrDefault(h => h.Key == "MerchantId" && h.Value == merchantId.ToString()).Value;

        var merchant = await _db.Merchants.FindAsync(merchantIdAsString);
        if (merchant == null)
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest, "Missing or invalid header 'MerchantId'");
        }
    }
}
