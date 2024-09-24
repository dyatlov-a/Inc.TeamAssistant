using Microsoft.JSInterop;

namespace Inc.TeamAssistant.WebUI.Extensions;

public static class JsRuntimeExtensions
{
    public static async ValueTask<TValue> Execute<TValue>(this IJSRuntime jsRuntime, IJsFunction<TValue> jsFunction)
    {
        ArgumentNullException.ThrowIfNull(jsRuntime);
        ArgumentNullException.ThrowIfNull(jsFunction);
        
        var result = await jsRuntime.InvokeAsync<TValue>(jsFunction.Identifier, jsFunction.Args);
        
        jsFunction.OnExecuted?.Invoke();
        
        return result;
    }
    
    public static async ValueTask Execute(this IJSRuntime jsRuntime, IJsFunction<dynamic> jsFunction)
    {
        ArgumentNullException.ThrowIfNull(jsRuntime);
        ArgumentNullException.ThrowIfNull(jsFunction);
        
        await jsRuntime.InvokeVoidAsync(jsFunction.Identifier, jsFunction.Args);
        
        jsFunction.OnExecuted?.Invoke();
    }
}