namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

public sealed class RequestState
{
    public bool IsLoading { get; }
    
    private RequestState(bool isLoading)
    {
        IsLoading = isLoading;
    }
    
    public static RequestState Loading() => new(isLoading: true);
    
    public static RequestState Done() => new(isLoading: false);
}