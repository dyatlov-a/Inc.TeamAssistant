using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;

namespace Inc.TeamAssistant.Gateway.ExceptionHandlers;

internal sealed class UnhandledExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(context);

        var statusCode = (int)HttpStatusCode.InternalServerError;
        var content = $$"""
                        {
                            "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                            "title": "Internal server error.",
                            "status": {{statusCode}},
                            "traceId": "{{context.TraceIdentifier}}",
                            "errors": {"message":["See logs for details."]}
                        }
                        """;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(content, token);

        return true;
    }
}