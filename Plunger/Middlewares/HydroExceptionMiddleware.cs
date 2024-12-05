using System.Net;
using Hydro;
using Microsoft.AspNetCore.Diagnostics;
using Plunger.Core;

namespace Plunger.Middlewares;

public class HydroExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public HydroExceptionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (!httpContext.IsHydro())
        {
            await _next(httpContext);
            return;
        }

        var contextFeature = httpContext.Features.Get<IExceptionHandlerFeature>();
        switch (contextFeature?.Error)
        {
            case DomainException domainException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new { domainException.Message });
                return;
            default:
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(
                    new UnhandledHydroError(
                        Message: "There was a problem with this operation and it wasn't finished",
                        Data: "Internal Server Error"
                    )
                );

                return;
        }
    }
}
