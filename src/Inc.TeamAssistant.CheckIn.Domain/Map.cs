namespace Inc.TeamAssistant.CheckIn.Domain;

public sealed class Map
{
    public Guid Id { get; private set; }
    public long ChatId { get; private set; }
    public Guid BotId { get; private set; }
    public string Name { get; private set; } = default!;

    private Map()
    {
    }

    public Map(Guid id, Guid botId, long chatId, string name)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Id = id;
        BotId = botId;
        ChatId = chatId;
        Name = name;
    }
}