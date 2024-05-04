namespace Inc.TeamAssistant.Connector.Domain;

public sealed class Bot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Token { get; private set; } = default!;
    
    private readonly List<BotCommand> _commands = new();
    public IReadOnlyCollection<BotCommand> Commands => _commands;
    
    private readonly List<Team> _teams = new();
    public IReadOnlyCollection<Team> Teams => _teams;

    public Bot AddCommand(BotCommand botCommand)
    {
        ArgumentNullException.ThrowIfNull(botCommand);

        _commands.Add(botCommand);

        return this;
    }
    
    public Bot AddTeam(Team team)
    {
        ArgumentNullException.ThrowIfNull(team);

        _teams.Add(team);

        return this;
    }
    
    public BotCommand? FindCommand(string cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(cmd));
        
        foreach (var botCommand in Commands)
            if (cmd.StartsWith(botCommand.Value, StringComparison.InvariantCultureIgnoreCase))
                return botCommand;

        return null;
    }
}