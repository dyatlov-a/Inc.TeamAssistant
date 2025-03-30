using Inc.TeamAssistant.Constructor.Model.Queries.GetProperties;
using Inc.TeamAssistant.Primitives.Features.Properties;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetProperties;

internal sealed class GetPropertiesQueryHandler : IRequestHandler<GetPropertiesQuery, GetPropertiesResult>
{
    private readonly IEnumerable<ISettingSectionProvider> _providers;

    public GetPropertiesQueryHandler(IEnumerable<ISettingSectionProvider> providers)
    {
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
    }

    public Task<GetPropertiesResult> Handle(GetPropertiesQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var providers = _providers.ToDictionary(p => p.FeatureName, p => p.GetSections());

        return Task.FromResult(new GetPropertiesResult(providers));
    }
}