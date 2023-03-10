using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;

public sealed record GetStoryDetailsQuery(
        AssessmentSessionId AssessmentSessionId,
        int Width,
        int Height,
        bool DrawQuietZones)
    : IRequest<GetStoryDetailsResult?>;