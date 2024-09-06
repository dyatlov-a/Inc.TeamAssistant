namespace Inc.TeamAssistant.WebUI.Features.Components;

public sealed class LoadingState
{
    public bool IsLoading { get; }
    
    private LoadingState(bool isLoading)
    {
        IsLoading = isLoading;
    }
    
    public static LoadingState Loading() => new(isLoading: true);
    
    public static LoadingState Done() => new(isLoading: false);
}