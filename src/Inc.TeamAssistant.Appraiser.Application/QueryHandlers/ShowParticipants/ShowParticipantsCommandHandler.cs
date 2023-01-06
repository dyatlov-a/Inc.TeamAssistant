using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowParticipants;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.ShowParticipants;

internal sealed class ShowParticipantsCommandHandler : IRequestHandler<ShowParticipantsQuery, ShowParticipantsResult>
{
    private readonly IAssessmentSessionRepository _repository;

    public ShowParticipantsCommandHandler(IAssessmentSessionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public Task<ShowParticipantsResult> Handle(ShowParticipantsQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        var assessmentSession = _repository.Find(query.AppraiserId).EnsureForAppraiser(query.AppraiserName);

		var appraisers = assessmentSession.Participants.Select(a => a.Name).ToArray();

        return Task.FromResult(new ShowParticipantsResult(assessmentSession.LanguageId, appraisers));
    }
}