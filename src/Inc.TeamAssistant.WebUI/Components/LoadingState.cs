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
    
    public static IProgress<State> Wrap(IProgress<State> innerState, Action updateView)
    {
        ArgumentNullException.ThrowIfNull(innerState);
        ArgumentNullException.ThrowIfNull(updateView);
        
        return new Progress<State>(s =>
        {
            innerState.Report(s);
            updateView.Invoke();
        });
    }
    
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