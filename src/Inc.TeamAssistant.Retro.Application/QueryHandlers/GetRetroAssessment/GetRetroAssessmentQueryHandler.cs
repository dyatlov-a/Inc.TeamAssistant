using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroAssessment;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroAssessment;

internal sealed class GetRetroAssessmentQueryHandler
    : IRequestHandler<GetRetroAssessmentQuery, GetRetroAssessmentResult>
{
    private readonly IPersonResolver _personResolver;
    private readonly IRetroAssessmentReader _reader;

    public GetRetroAssessmentQueryHandler(IPersonResolver personResolver, IRetroAssessmentReader reader)
    {
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetRetroAssessmentResult> Handle(GetRetroAssessmentQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var assessment = await _reader.Read(query.SessionId, currentPerson.Id, token);

        return new GetRetroAssessmentResult(assessment.RoomId, assessment.Value ?? 0);
    }
}