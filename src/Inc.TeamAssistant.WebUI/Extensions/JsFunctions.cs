using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Extensions;

internal static class JsFunctions
{
    public static IJsFunction<int> GetTimezone()
    {
        return new JsFunction<int>(
            "window.browserJsFunctions.getTimezone",
            postAction: null,
            args: null);
    }

    public static IJsFunction<string> GetTextValue(ElementReference element)
    {
        return new JsFunction<string>(
            "window.browserJsFunctions.editableText.get",
            postAction: null,
            element);
    }
    
    public static IJsFunction<dynamic> SetTextValue(ElementReference element, string? value)
    {
        return new JsFunction<dynamic>(
            "window.browserJsFunctions.editableText.set",
            postAction: null,
            element,
            value);
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

    public static IJsFunction<dynamic> ScrollToTop()
    {
        return new JsFunction<dynamic>(
            "window.browserJsFunctions.scrollToTop",
            postAction: null);
    }
    
    public static IJsFunction<dynamic> FocusElement(ElementReference element)
    {
        return new JsFunction<dynamic>(
            "window.browserJsFunctions.focusElement",
            postAction: null,
            element);
    }
    
    public static IJsFunction<dynamic> AddClassToElement(string selector, string className)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        ArgumentException.ThrowIfNullOrWhiteSpace(className);
        
        return new JsFunction<dynamic>(
            "window.browserJsFunctions.addClassToElement",
            postAction: null,
            selector,
            className);
    }
    
    public static IJsFunction<dynamic> AddPageScrollObserver(
        string elementSelector,
        string anchorSelector,
        string modificatorClass)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(elementSelector);
        ArgumentException.ThrowIfNullOrWhiteSpace(anchorSelector);
        ArgumentException.ThrowIfNullOrWhiteSpace(modificatorClass);
        
        return new JsFunction<dynamic>(
            "window.browserJsFunctions.addPageScrollObserver",
            postAction: null,
            elementSelector,
            anchorSelector,
            modificatorClass);
    }
    
    public static IJsFunction<dynamic> AddCleanStyleActionToPasteListener(ElementReference element)
    {
        return new JsFunction<dynamic>(
            "window.browserJsFunctions.addCleanStyleActionToPasteListener",
            postAction: null,
            element);
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