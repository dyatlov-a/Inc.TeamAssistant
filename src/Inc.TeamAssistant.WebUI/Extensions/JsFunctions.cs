using Inc.TeamAssistant.WebUI.Services.ClientCore;

namespace Inc.TeamAssistant.WebUI.Extensions;

public static class JsFunctions
{
    public static IJsFunction<int> GetTimezone()
    {
        return new JsFunction<int>("window.browserJsFunctions.getTimezone", postAction: null, args: null);
    }
    
    public static IJsFunction<dynamic> ChangeUrl(NavRoute route, Action<NavRoute> onChanged)
    {
        ArgumentNullException.ThrowIfNull(route);
        ArgumentNullException.ThrowIfNull(onChanged);

        return new JsFunction<dynamic>(
            "window.browserJsFunctions.changeUrl",
            () => onChanged(route),
            route.ToString());
    }
    
    private sealed record JsFunction<TResult> : IJsFunction<TResult>
    {
        public string Identifier { get; }
        public Action? PostAction { get; }
        public object?[]? Args { get; }

        internal JsFunction(string identifier, Action? postAction, params object?[]? args)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        
            Identifier = identifier;
            PostAction = postAction;
            Args = args;
        }
    }
}