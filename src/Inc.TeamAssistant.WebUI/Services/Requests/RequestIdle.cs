namespace Inc.TeamAssistant.WebUI.Services.Requests;

public sealed class RequestIdle
{
    private RequestIdle()
    {
    }
    
    public static readonly RequestIdle Idle = new();
}