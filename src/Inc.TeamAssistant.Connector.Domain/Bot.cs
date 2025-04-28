namespace Inc.TeamAssistant.Connector.Domain;

public sealed class Bot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Token { get; private set; } = default!;
    public long OwnerId { get; private set; }
    
    private readonly List<ContextCommand> _commands = new();
    public IReadOnlyCollection<ContextCommand> Commands => _commands;
    
    private readonly List<Team> _teams = new();
    public IReadOnlyCollection<Team> Teams => _teams;
    public IReadOnlyDictionary<string, string> Properties { get; private set; } = new Dictionary<string, string>();

    public Bot AddCommand(ContextCommand contextCommand)
    {
        ArgumentNullException.ThrowIfNull(contextCommand);

        _commands.Add(contextCommand);

        return this;
    }
    
    public Bot AddTeam(Team team)
    {
        ArgumentNullException.ThrowIfNull(team);

        _teams.Add(team);

        return this;
    }
    
    public ContextCommand? FindCommand(string cmd)
    {
        ArgumentNullException.ThrowIfNull(cmd);
        
        foreach (var botCommand in Commands)
            if (cmd.StartsWith(botCommand.Value, StringComparison.InvariantCultureIgnoreCase))
                return botCommand;

        return null;
    }
}