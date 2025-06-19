using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.WebUI.Services.Internal;

internal sealed class ClientException : TeamAssistantException
{
    public ErrorDetails Detail { get; }
    
    public ClientException(ErrorDetails detail)
        : base(detail.Title)
    {
        Detail = detail;
    }
}