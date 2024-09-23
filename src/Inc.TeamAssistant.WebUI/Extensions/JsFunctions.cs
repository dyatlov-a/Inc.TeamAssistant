using Inc.TeamAssistant.WebUI.Services.ClientCore;

namespace Inc.TeamAssistant.WebUI.Extensions;

public static class JsFunctions
{
    public static IJsFunction<int> GetTimezone()
    {
        return new JsFunction<int>("window.browserJsFunctions.getTimezone", postAction: null, args: null);
    }
    
    public static IJsFunction<dynamic> ChangeUrl(this NavRouter navRouter, string url)
    {
        ArgumentNullException.ThrowIfNull(navRouter);
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        
        return new JsFunction<dynamic>("window.browserJsFunctions.changeUrl", () => navRouter.ChangeRoute(url), url);
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