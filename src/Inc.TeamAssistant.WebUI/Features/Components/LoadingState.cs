namespace Inc.TeamAssistant.WebUI.Features.Components;

public sealed class LoadingState
{
    public State Value { get; }
    
    private LoadingState(State value)
    {
        Value = value;
    }
    
    public static LoadingState Loading() => new(State.Loading);
    
    public static LoadingState Error() => new(State.Error);
    
    public static LoadingState Done() => new(State.Done);
    
    public enum State
    {
        Loading = 1,
        Error = 2,
        Done = 3
    }
}