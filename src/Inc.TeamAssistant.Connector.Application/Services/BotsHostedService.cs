using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives.Bots;
using Microsoft.Extensions.Hosting;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class BotsHostedService : IHostedService
{
    private readonly IBotReader _botReader;
    private readonly IBotListeners _botListeners;
    
    public BotsHostedService(IBotReader botReader, IBotListeners botListeners)
    {
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
        _botListeners = botListeners ?? throw new ArgumentNullException(nameof(botListeners));
    }

    public async Task StartAsync(CancellationToken token)
    {
        foreach (var botId in await _botReader.GetBotIds(token))
        {
            token.ThrowIfCancellationRequested();
            
            await _botListeners.Start(botId, token);
        }
    }
    
    public async Task StopAsync(CancellationToken token) => await _botListeners.Shutdown(token);
}