using Inc.TeamAssistant.Appraiser.Model;
using Microsoft.JSInterop;

namespace Inc.TeamAssistant.WebUI.Services.Render;

internal sealed class VideoClientService : IVideoService
{
    private readonly IJSRuntime _jsRuntime;

    public VideoClientService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }
    
    public async Task Play(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));
        
        await _jsRuntime.InvokeAsync<string>("videoPlay", id);
    }

    public bool IsServer => false;
}