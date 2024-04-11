using Inc.TeamAssistant.Appraiser.Model;

namespace Inc.TeamAssistant.Gateway.Services;

internal sealed class VideoServerService : IVideoService
{
    public bool IsServer => true;
    
    public Task Play(string id) => Task.CompletedTask;
}