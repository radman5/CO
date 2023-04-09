using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CO.Payments.Api.Controllers.ExceptionHandling;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is HttpResponseException responseException)
        {
            context.Result = new ObjectResult(responseException.Value)
            {
                StatusCode = Convert.ToInt32(responseException.StatusCode),
                Value = responseException.Message
            };

            context.ExceptionHandled = true;
        }
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }
}
