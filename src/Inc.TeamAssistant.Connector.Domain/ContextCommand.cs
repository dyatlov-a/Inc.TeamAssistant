using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Connector.Domain;

public sealed class ContextCommand
{
    public Guid Id { get; private set; }
    public string Value { get; private set; } = default!;
    public MessageId? HelpMessageId { get; private set; }
    public IReadOnlyCollection<ContextScope> Scopes { get; private set; } = default!;
    public ContextStages Stages { get; private set; } = default!;

    internal ContextCommand MapStages(IEnumerable<ContextStage> contextStages)
    {
        ArgumentNullException.ThrowIfNull(contextStages);
        
        Stages = new(contextStages);

        return this;
    }
}