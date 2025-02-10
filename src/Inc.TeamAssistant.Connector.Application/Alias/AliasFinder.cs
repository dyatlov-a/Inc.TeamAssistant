using System.Reflection;

namespace Inc.TeamAssistant.Connector.Application.Alias;

public static class AliasFinder
{
    public static IEnumerable<AliasValue> Find()
    {
        var fields = Assembly.GetExecutingAssembly()
            .GetTypes()
            .SelectMany(t => t.GetFields());

        foreach (var field in fields)
        foreach (var attribute in field.GetCustomAttributes())
            if (attribute is CommandAlias commandAlias)
                yield return new AliasValue(commandAlias.Value, (string)field.GetValue(null)!);
    }
}