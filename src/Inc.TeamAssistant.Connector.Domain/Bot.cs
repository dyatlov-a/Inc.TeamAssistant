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

    private Bot()
    {
    }
    
    public Bot(string name, string token)
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));
        
        Id = Guid.NewGuid();
        Name = name;
        Token = token;
    }

    public Bot AddCommand(BotCommand botCommand)
    {
        if (botCommand is null)
            throw new ArgumentNullException(nameof(botCommand));
        
        _commands.Add(botCommand);

        return this;
    }
    
    public Bot AddTeam(Team team)
    {
        if (team is null)
            throw new ArgumentNullException(nameof(team));
        
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