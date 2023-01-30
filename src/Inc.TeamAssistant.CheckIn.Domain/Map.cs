using Inc.TeamAssistant.CheckIn.Primitives;

namespace Inc.TeamAssistant.CheckIn.Domain;

public sealed class Map
{
    public MapId Id { get; private set; } = default!;
    public long ChatId { get; private set; }

    private Map()
    {
    }

    public Map(long chatId)
        : this()
    {
        Id = new MapId(Guid.NewGuid());
        ChatId = chatId;
    }
}