namespace Inc.TeamAssistant.CheckIn.Domain;

public sealed class Map
{
    public Guid Id { get; private set; }
    public long ChatId { get; private set; }

    private Map()
    {
    }

    public Map(long chatId)
        : this()
    {
        Id = Guid.NewGuid();
        ChatId = chatId;
    }
}