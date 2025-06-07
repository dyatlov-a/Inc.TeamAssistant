using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroAssessment;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroAssessment;

internal sealed class GetRetroAssessmentQueryHandler
    : IRequestHandler<GetRetroAssessmentQuery, GetRetroAssessmentResult>
{
    private readonly IPersonResolver _personResolver;
    private readonly IRetroAssessmentRepository _repository;

    public GetRetroAssessmentQueryHandler(IPersonResolver personResolver, IRetroAssessmentRepository repository)
    {
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<GetRetroAssessmentResult> Handle(GetRetroAssessmentQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var assessment = await _repository.Find(query.SessionId, currentPerson.Id, token);

        return new GetRetroAssessmentResult(assessment?.Value ?? 0);
    }
}