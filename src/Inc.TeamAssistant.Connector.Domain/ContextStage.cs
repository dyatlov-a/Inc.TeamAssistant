using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Connector.Domain;

public sealed class ContextStage
{
    public Guid Id { get; private set; }
    public Guid BotCommandId { get; private set; }
    public StageType Value { get; private set; }
    public MessageId DialogMessageId { get; private set; } = default!;
    public int Position { get; private set; }
}