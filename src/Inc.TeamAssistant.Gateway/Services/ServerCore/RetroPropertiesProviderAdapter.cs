using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Tenants.Application.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class RetroPropertiesProviderAdapter : IRetroPropertiesProvider
{
    private const string FacilitatorIdFieldName = nameof(RetroProperties.FacilitatorId);
    
    private readonly IRoomPropertiesProvider _provider;

    public RetroPropertiesProviderAdapter(IRoomPropertiesProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public async Task<RetroProperties> Get(Guid roomId, CancellationToken token)
    {
        var properties = await _provider.Get(roomId, token);

        var retroProperties = new RetroProperties();
        
        if (properties.TryGetValue(FacilitatorIdFieldName, out var facilitatorIdValue) &&
            long.TryParse(facilitatorIdValue, out var facilitatorId))
            retroProperties.FacilitatorId = facilitatorId;
        
        return retroProperties;
    }

    public async Task Set(Guid roomId, RetroProperties properties, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(properties);
        
        var currentProperties = await _provider.Get(roomId, token);
        var newProperties = currentProperties.ToDictionary(StringComparer.InvariantCultureIgnoreCase);

        if (properties.FacilitatorId.HasValue)
            newProperties[FacilitatorIdFieldName] = properties.FacilitatorId.Value.ToString();
        else
            newProperties.Remove(FacilitatorIdFieldName);

        await _provider.Set(roomId, newProperties, token);
    }
}