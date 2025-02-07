namespace Inc.TeamAssistant.Connector.Application.Alias;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class CommandAlias : Attribute
{
    public string Value { get; }

    public CommandAlias(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value;
    }
}