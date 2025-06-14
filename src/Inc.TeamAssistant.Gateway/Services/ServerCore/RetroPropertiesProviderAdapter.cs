using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Tenants.Application.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class RetroPropertiesProviderAdapter : IRetroPropertiesProvider
{
    private readonly IRoomPropertiesProvider _provider;

    public RetroPropertiesProviderAdapter(IRoomPropertiesProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public async Task<RetroProperties> Get(Guid roomId, CancellationToken token)
    {
        return await _provider.Get<RetroProperties>(roomId, token);
    }

    public async Task Set(Guid roomId, RetroProperties properties, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(properties);
        
        var newProperties = await _provider.Get<RetroProperties>(roomId, token);

        if (properties.FacilitatorId.HasValue)
            newProperties.FacilitatorId = properties.FacilitatorId.Value;
        if (properties.TemplateId.HasValue)
            newProperties.TemplateId = properties.TemplateId.Value;
        if (properties.TimerDuration.HasValue)
            newProperties.TimerDuration = properties.TimerDuration.Value;
        if (properties.VoteCount.HasValue)
            newProperties.VoteCount = properties.VoteCount.Value;

        await _provider.Set(roomId, newProperties, token);
    }
}