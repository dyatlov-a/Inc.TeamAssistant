using Inc.TeamAssistant.Primitives.Extensions;

namespace Inc.TeamAssistant.Connector.Application.Alias;

internal sealed class AliasService
{
    private readonly IReadOnlyDictionary<string, string> _aliasMap;

    public AliasService(IEnumerable<AliasValue> aliasMap)
    {
        ArgumentNullException.ThrowIfNull(aliasMap);

        _aliasMap = aliasMap.ToDictionary(i => i.Alias, i => i.Command, StringComparer.InvariantCultureIgnoreCase);
    }
    
    public string OverrideCommand(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (!text.HasCommand())
            return text;
        
        var alias = text.Split(' ').First();
        var result = _aliasMap.TryGetValue(alias, out var command)
            ? text.Replace(alias, command)
            : text;

        return result;
    }
}