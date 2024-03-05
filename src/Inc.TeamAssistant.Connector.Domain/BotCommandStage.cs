using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Domain;

public sealed class BotCommandStage
{
    public Guid Id { get; private set; }
    public Guid BotCommandId { get; private set; }
    public CommandStage Value { get; private set; }
    public MessageId DialogMessageId { get; private set; } = default!;
    public int Position { get; private set; }

    private BotCommandStage()
    {
    }

    public BotCommandStage(Guid botCommandId, CommandStage value, MessageId dialogMessageId, int position)
        : this()
    {
        Id = Guid.NewGuid();
        BotCommandId = botCommandId;
        Value = value;
        DialogMessageId = dialogMessageId;
        Position = position;
    }
}