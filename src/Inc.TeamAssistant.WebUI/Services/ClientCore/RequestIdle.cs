namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

public sealed class RequestIdle
{
    private RequestIdle()
    {
    }
    
    public static readonly RequestIdle Idle = new();
}