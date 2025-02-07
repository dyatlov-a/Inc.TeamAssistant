namespace Inc.TeamAssistant.Connector.Application.Alias;

public sealed class CommandAlias : Attribute
{
    public AliasValue Value { get; }

    public CommandAlias(string alias, string command)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);
        ArgumentException.ThrowIfNullOrWhiteSpace(command);

        Value = new(alias, command);
    }
}