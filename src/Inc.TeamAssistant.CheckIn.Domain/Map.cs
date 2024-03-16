namespace Inc.TeamAssistant.CheckIn.Domain;

public sealed class Map
{
    public Guid Id { get; private set; }
    public long ChatId { get; private set; }
    public Guid BotId { get; private set; }

    private Map()
    {
    }

    public Map(Guid botId, long chatId)
        : this()
    {
        Id = Guid.NewGuid();
        BotId = botId;
        ChatId = chatId;
    }
}