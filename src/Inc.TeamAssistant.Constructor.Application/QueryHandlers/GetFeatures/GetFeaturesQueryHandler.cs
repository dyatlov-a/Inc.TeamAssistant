using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetFeatures;

internal sealed class GetFeaturesQueryHandler : IRequestHandler<GetFeaturesQuery, GetFeaturesResult>
{
    private readonly IFeatureReader _reader;

    public GetFeaturesQueryHandler(IFeatureReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetFeaturesResult> Handle(GetFeaturesQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var features = await _reader.GetAll(token);
        
        return new GetFeaturesResult(features);
    }
}