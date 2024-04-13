namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class AliasService
{
    private readonly IReadOnlyDictionary<string, string> _aliases = new Dictionary<string, string>(
        StringComparer.CurrentCultureIgnoreCase)
    {
        ["/nr"] = "/need_review"
    };
    
    public string OverrideCommand(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

        if (text.StartsWith('/'))
        {
            var alias = text.Split(' ').First();
        
            if (_aliases.TryGetValue(alias, out var command))
                return text.Replace(alias, command);
        }
        
        return text;
    }
}