using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Connector.Domain;

public sealed class ContextCommand
{
    public Guid Id { get; private set; }
    public string Value { get; private set; } = default!;
    public MessageId? HelpMessageId { get; private set; }
    public IReadOnlyCollection<ContextScope> Scopes { get; private set; } = Array.Empty<ContextScope>();

    private readonly List<ContextStage> _stages = new();
    public IReadOnlyCollection<ContextStage> Stages => _stages.OrderBy(s => s.Position).ToArray();

    public ContextCommand AddStage(ContextStage contextStage)
    {
        ArgumentNullException.ThrowIfNull(contextStage);

        _stages.Add(contextStage);

        return this;
    }
}