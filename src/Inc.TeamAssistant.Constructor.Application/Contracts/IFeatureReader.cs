using Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;

namespace Inc.TeamAssistant.Constructor.Application.Contracts;

public interface IFeatureReader
{
    Task<IReadOnlyCollection<FeatureDto>> GetAll(CancellationToken token);
}