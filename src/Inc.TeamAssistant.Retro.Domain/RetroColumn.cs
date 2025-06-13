namespace Inc.TeamAssistant.Retro.Domain;

public sealed class RetroColumn
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public int Position { get; private set; }
    public string Color { get; private set; } = default!;
    public string? Description { get; private set; }

    private RetroColumn()
    {
    }

    public RetroColumn(Guid id, string name, int position, string color, string? description)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(color);
        
        Id = id;
        Name = name;
        Position = position;
        Color = color;
        Description = description;
    }
}