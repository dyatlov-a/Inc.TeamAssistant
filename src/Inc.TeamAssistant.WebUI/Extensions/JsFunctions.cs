namespace Inc.TeamAssistant.WebUI.Extensions;

public sealed class JsFunctions
{
    public string Identifier { get; }
    public object?[]? Args { get; }

    private JsFunctions(string identifier, params object?[]? args)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        
        Identifier = identifier;
        Args = args;
    }

    public static JsFunctions GetTimezone() => new("window.browserJsFunctions.getTimezone", null);
    
    public static JsFunctions ChangeUrl(string url)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        
        return new JsFunctions("window.browserJsFunctions.changeUrl", url);
    }
}