namespace Inc.TeamAssistant.WebUI.Services.Internal;

internal sealed class DragAndDropService<T>
    where T : class
{
    public T? Current { get; private set; }
    
    public void Start(T current)
    {
        Current = current;
    }

    public T? End()
    {
        if (Current is null)
            return null;
        
        var current = Current;
        
        Current = null;
        
        return current; 
    }
}