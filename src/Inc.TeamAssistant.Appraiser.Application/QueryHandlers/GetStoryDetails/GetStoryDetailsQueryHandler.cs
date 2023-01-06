using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetStoryDetails;

internal sealed class GetStoryDetailsQueryHandler : IRequestHandler<GetStoryDetailsQuery, GetStoryDetailsResult?>
{
	private readonly IAssessmentSessionRepository _repository;
    private readonly IQuickResponseCodeGenerator _codeGenerator;
    private readonly ILinkBuilder _linkBuilder;

	public GetStoryDetailsQueryHandler(
        IAssessmentSessionRepository repository,
        IQuickResponseCodeGenerator codeGenerator,
        ILinkBuilder linkBuilder)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
        _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
	}

	public Task<GetStoryDetailsResult?> Handle(GetStoryDetailsQuery query, CancellationToken cancellationToken)
	{
		if (query is null)
			throw new ArgumentNullException(nameof(query));

		var assessmentSession = _repository.Find(query.AssessmentSessionId);

        var result = assessmentSession is not null
            ? Get(assessmentSession, query.Width, query.Height, query.DrawQuietZones)
            : null;

        return Task.FromResult(result);
    }

    private GetStoryDetailsResult Get(AssessmentSession assessmentSession, int width, int height, bool drawQuietZones)
    {
        if (assessmentSession is null)
            throw new ArgumentNullException(nameof(assessmentSession));

        var estimateEnded = assessmentSession.EstimateEnded();
        var link = _linkBuilder.BuildLinkForConnect(assessmentSession.Id);
        var code = _codeGenerator.Generate(link, width, height, drawQuietZones);

        var items = assessmentSession.CurrentStory.StoryForEstimates
            .Select(e => new StoryForEstimateDto(
                e.Participant.Name,
                estimateEnded ? e.Value.ToDisplayValue() : e.Value.ToDisplayHasValue()))
            .ToArray();

        return new(
            assessmentSession.Title,
            code,
            StorySelected: assessmentSession.CurrentStory != Story.Empty,
            StoryConverter.ConvertTo(assessmentSession.CurrentStory),
            items,
            assessmentSession.CurrentStory.GetTotal().ToDisplayValue(estimateEnded));
    }
}