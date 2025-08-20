using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Diagnostics;

namespace Inc.TeamAssistant.Gateway.ExceptionHandlers;

internal sealed class TeamAssistantUserExceptionHandler : IExceptionHandler
{
    private readonly IRenderContext _renderContext;
    private readonly IMessageBuilder _messageBuilder;

    public TeamAssistantUserExceptionHandler(IRenderContext renderContext, IMessageBuilder messageBuilder)
    {
        _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (exception is not TeamAssistantUserException userException)
            return false;

        var statusCode = (int)HttpStatusCode.BadRequest;
        var errorMessage = _messageBuilder.Build(
            userException.MessageId,
            _renderContext.CurrentLanguage,
            userException.Values);
        var errorDetails = new ErrorDetails(
            "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            "Bad Request.",
            statusCode,
            context.TraceIdentifier,
            new Dictionary<string, string[]>
            {
                {
                    "message", [errorMessage]
                }
            });

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorDetails), token);
        
        return true;
    }
}