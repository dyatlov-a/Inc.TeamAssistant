namespace Inc.TeamAssistant.WebUI.Components;

public sealed class LoadingState : IProgress<LoadingState.State>
{
    public State Value { get; private set; }
    
    private LoadingState(State value)
    {
        Value = value;
    }
    
    public static LoadingState Loading() => new(State.Loading);
    
    public static LoadingState Error() => new(State.Error);
    
    public static LoadingState Done() => new(State.Done);
    
    public void Report(State value)
    {
        Value = value;
    }
    
    public enum State
    {
        Loading = 1,
        Error = 2,
        Done = 3
    }
}