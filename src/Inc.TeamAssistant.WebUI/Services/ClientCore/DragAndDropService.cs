namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

internal sealed class DragAndDropService<T>
{
    public T? Current { get; private set; }
    
    public void Start(T current)
    {
        Current = current;
    }

    public T End()
    {
        if (Current is null)
            throw new InvalidOperationException("Drag and drop operation has not been started.");
        
        var current = Current;
        Current = default;
        
        return current; 
    }
}