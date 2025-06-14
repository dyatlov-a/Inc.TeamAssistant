namespace Inc.TeamAssistant.Retro.Domain;

public sealed class RetroTemplate
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;

    private RetroTemplate()
    {
    }

    public RetroTemplate(Guid id, string name)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Id = id;
        Name = name;
    }
}