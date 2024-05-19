namespace Inc.TeamAssistant.Connector.Domain;

public sealed class Photo
{
    public Guid Id { get; private set; }
    public long PersonId { get; private set; }
    public DateTimeOffset Date { get; private set; }
    public byte[] Data { get; private set; } = default!;

    private Photo()
    {
    }

    public Photo(long personId)
        : this()
    {
        Id = Guid.NewGuid();
        PersonId = personId;
    }

    public void SetData(byte[] data)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
        Date = DateTimeOffset.UtcNow;
    }
}