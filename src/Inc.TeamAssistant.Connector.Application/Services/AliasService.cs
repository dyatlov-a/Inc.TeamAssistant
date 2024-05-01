namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class AliasService
{
    private readonly IReadOnlyDictionary<string, string> _aliases = new Dictionary<string, string>(
        StringComparer.CurrentCultureIgnoreCase)
    {
        ["/nr"] = "/need_review",
        ["/l"] = "/location"
    };
    
    public string OverrideCommand(string text)
    {
        if (text.StartsWith('/'))
        {
            var alias = text.Split(' ').First();
        
            if (_aliases.TryGetValue(alias, out var command))
                return text.Replace(alias, command);
        }
        
        return text;
    }
}