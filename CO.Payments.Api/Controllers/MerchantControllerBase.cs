using CO.Payments.Api.Controllers.ExceptionHandling;
using CO.Payments.Api.Data.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace CO.Payments.Api.Controllers;

public abstract class MerchantControllerBase : ControllerBase
{
    protected static async Task ValidateMerchantExists(PaymentsDbContext _db, HttpRequest request, long merchantId)
    {
        if (request.Headers.Any(h => h.Key == "MerchantId" && h.Value == merchantId.ToString()))
        {
            var merchant = await _db.Merchants.FindAsync(merchantId);
            if (merchant == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Missing or invalid header 'MerchantId'");
            }
        }
        else
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest, "Missing or invalid header 'MerchantId'");
        }
    }
}
