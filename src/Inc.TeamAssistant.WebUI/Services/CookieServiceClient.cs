using Inc.TeamAssistant.Appraiser.Model;
using Microsoft.JSInterop;

namespace Inc.TeamAssistant.WebUI.Services;

internal sealed class CookieServiceClient : ICookieService
{
    private readonly IJSRuntime _jsRuntime;

    public CookieServiceClient(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    public bool IsServerRender => false;

    public async Task<string?> GetValue(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

        return await _jsRuntime.InvokeAsync<string>("readCookie", name);
    }

    public async Task SetValue(string name, string value, int lifetimeInDays)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

        await _jsRuntime.InvokeAsync<string>("writeCookie", name, value, lifetimeInDays);
    }
}