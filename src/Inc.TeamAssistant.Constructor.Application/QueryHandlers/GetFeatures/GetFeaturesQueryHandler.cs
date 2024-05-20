using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetFeatures;

internal sealed class GetFeaturesQueryHandler : IRequestHandler<GetFeaturesQuery, GetFeaturesResult>
{
    private readonly IFeatureReader _featureReader;

    public GetFeaturesQueryHandler(IFeatureReader featureReader)
    {
        _featureReader = featureReader ?? throw new ArgumentNullException(nameof(featureReader));
    }

    public async Task<GetFeaturesResult> Handle(GetFeaturesQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var features = await _featureReader.GetAll(token);
        
        return new GetFeaturesResult(features);
    }
}