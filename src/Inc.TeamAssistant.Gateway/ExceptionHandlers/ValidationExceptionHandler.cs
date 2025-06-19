using System.Net;
using System.Net.Mime;
using System.Text.Json;
using FluentValidation;
using Inc.TeamAssistant.Primitives.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Inc.TeamAssistant.Gateway.ExceptionHandlers;

internal sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (exception is not ValidationException validationException)
            return false;
        
        var statusCode = (int)HttpStatusCode.BadRequest;
        var errors = validationException.Errors
            .Select(e => new { e.PropertyName, e.ErrorMessage })
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).OrderBy(e => e).ToArray());
        var errorDetails = new ErrorDetails(
            "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            "Bad Request.",
            statusCode,
            context.TraceIdentifier,
            errors);

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorDetails), token);
            
        return true;
    }
}