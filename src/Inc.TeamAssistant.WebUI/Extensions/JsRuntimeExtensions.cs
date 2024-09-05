using Microsoft.JSInterop;

namespace Inc.TeamAssistant.WebUI.Extensions;

public static class JsRuntimeExtensions
{
    public static async ValueTask<TValue> Execute<TValue>(this IJSRuntime jsRuntime, JsFunctions jsFunction)
    {
        ArgumentNullException.ThrowIfNull(jsRuntime);
        ArgumentNullException.ThrowIfNull(jsFunction);
        
        return await jsRuntime.InvokeAsync<TValue>(jsFunction.Identifier, jsFunction.Args);
    }
    
    public static async ValueTask Execute(this IJSRuntime jsRuntime, JsFunctions jsFunction)
    {
        ArgumentNullException.ThrowIfNull(jsRuntime);
        ArgumentNullException.ThrowIfNull(jsFunction);
        
        await jsRuntime.InvokeVoidAsync(jsFunction.Identifier, jsFunction.Args);
    }
}