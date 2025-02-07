using System.Reflection;

namespace Inc.TeamAssistant.Connector.Application.Alias;

public static class AliasFactory
{
    public static IEnumerable<AliasValue> Create()
    {
        var aliases = Assembly.GetExecutingAssembly()
            .GetTypes()
            .SelectMany(t => t.GetFields().SelectMany(f => f.GetCustomAttributes()))
            .OfType<CommandAlias>();

        foreach (var alias in aliases)
            yield return alias.Value;
    }
}