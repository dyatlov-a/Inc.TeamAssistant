namespace Inc.TeamAssistant.WebUI.Extensions;

public static class JsFunctions
{
    public static IJsFunction<int> GetTimezone()
    {
        return new JsFunction<int>("window.browserJsFunctions.getTimezone", postAction: null, args: null);
    }
    
    public static IJsFunction<dynamic> ChangeUrl(string url, Action<string> onChanged)
    {
        ArgumentNullException.ThrowIfNull(url);
        ArgumentNullException.ThrowIfNull(onChanged);

        return new JsFunction<dynamic>(
            "window.browserJsFunctions.changeUrl",
            () => onChanged(url),
            url);
    }
    
    private sealed record JsFunction<TResult> : IJsFunction<TResult>
    {
        public string Identifier { get; }
        public Action? OnExecuted { get; }
        public object?[]? Args { get; }

        internal JsFunction(string identifier, Action? postAction, params object?[]? args)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        
            Identifier = identifier;
            OnExecuted = postAction;
            Args = args;
        }
    }
}