using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Inc.TeamAssistant.Gateway.ExceptionHandlers;

internal sealed class UnhandledExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(context);

        var statusCode = (int)HttpStatusCode.InternalServerError;
        var errorDetails = new ErrorDetails(
            "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            "Internal server error.",
            statusCode,
            context.TraceIdentifier,
            new Dictionary<string, string[]>
            {
                {
                    "message", ["See logs for details."]
                }
            });

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorDetails), token);

        return true;
    }
}