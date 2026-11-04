using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NIHR.Infrastructure.AspNetCore.TrustedUris;

internal class UntrustedUriExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is UntrustedUriException)
        {
            context.ExceptionHandled = true;
            context.Result = new BadRequestResult();
        }
    }
}