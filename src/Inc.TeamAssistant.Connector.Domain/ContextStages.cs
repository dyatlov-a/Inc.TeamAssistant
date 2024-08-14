namespace Inc.TeamAssistant.Connector.Domain;

public sealed class ContextStages
{
    private readonly LinkedList<ContextStage> _stages;
    private LinkedListNode<ContextStage>? _current;

    public ContextStages(IEnumerable<ContextStage> stages)
    {
        ArgumentNullException.ThrowIfNull(stages);
        
        _stages = new(stages.OrderBy(s => s.Position));
    }
    
    public ContextStage Current => GetCurrent().Value;
    public ContextStage? Next => GetCurrent().Next?.Value;
    public bool IsFirst => Current.Id == _stages.First!.Value.Id;
    
    public bool MoveNext()
    {
        if (_stages.First is null)
            return false;
        
        if (_current is null)
        {
            _current = _stages.First;
            return true;
        }

        if (_current.Next is null)
            return false;

        _current = _current.Next;
        return true;
    }

    private LinkedListNode<ContextStage> GetCurrent()
    {
        if (_current is null)
            throw new InvalidOperationException(
                $"Current stage is not set. Please call {nameof(MoveNext)} method first and check result.");
        
        return _current;
    }
}