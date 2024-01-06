using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Domain;

public sealed class BotCommand
{
    public Guid Id { get; private set; }
    public string Value { get; private set; } = default!;
    public MessageId? HelpMessageId { get; private set; }

    private readonly List<BotCommandStage> _stages = new();
    public IReadOnlyCollection<BotCommandStage> Stages => _stages;

    private BotCommand()
    {
    }
    
    public BotCommand(string value, MessageId? helpMessageId)
        : this()
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        
        Id = Guid.NewGuid();
        Value = value;
        HelpMessageId = helpMessageId;
    }

    public BotCommand AddStage(BotCommandStage stage)
    {
        _stages.Add(stage);

        return this;
    }
}