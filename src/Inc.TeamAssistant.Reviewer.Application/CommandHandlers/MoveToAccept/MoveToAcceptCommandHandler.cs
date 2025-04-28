using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept;

internal sealed class MoveToAcceptCommandHandler : IRequestHandler<MoveToAcceptCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _repository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;
    private readonly IReviewMetricsProvider _metricsProvider;
    private readonly INextReviewerStrategyFactory _reviewerFactory;
    private readonly ITeamAccessor _teamAccessor;
    private readonly ReviewCommentsProvider _commentsProvider;

    public MoveToAcceptCommandHandler(
        ITaskForReviewRepository repository,
        IReviewMessageBuilder reviewMessageBuilder,
        IReviewMetricsProvider metricsProvider,
        INextReviewerStrategyFactory reviewerFactory,
        ITeamAccessor teamAccessor,
        ReviewCommentsProvider commentsProvider)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
        _metricsProvider = metricsProvider ?? throw new ArgumentNullException(nameof(metricsProvider));
        _reviewerFactory = reviewerFactory ?? throw new ArgumentNullException(nameof(reviewerFactory));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _commentsProvider = commentsProvider ?? throw new ArgumentNullException(nameof(commentsProvider));
    }

    public async Task<CommandResult> Handle(MoveToAcceptCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var taskForReview = await command.TaskId.Required(_repository.Find, token);
        if (!taskForReview.CanMakeDecision())
            return CommandResult.Empty;

        var nextReviewerId = await GetSecondRoundReviewer(taskForReview, token);

        await _repository.Upsert(taskForReview.FinishRound(DateTimeOffset.UtcNow, nextReviewerId), token);

        if (taskForReview.CanMakeDecision())
            _commentsProvider.Add(taskForReview);
        else
            _commentsProvider.Remove(taskForReview);

        var notifications = await _reviewMessageBuilder.Build(taskForReview, fromOwner: false, token);
        await _metricsProvider.Add(taskForReview, token);
        return CommandResult.Build(notifications.ToArray());
    }

    private async Task<long?> GetSecondRoundReviewer(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        
        var finalizes = await _teamAccessor.GetFinalizes(taskForReview.TeamId, DateTimeOffset.UtcNow, token);
        if (!finalizes.Any())
            return null;

        var nextReviewerStrategy = await _reviewerFactory.Create(
            taskForReview.TeamId,
            taskForReview.OwnerId,
            taskForReview.ReviewerId,
            NextReviewerType.SecondRound,
            targetPersonId: null,
            finalizes.Select(t => t.Id).ToArray(),
            token);
        
        var result = nextReviewerStrategy.GetReviewer();
        return result;
    }
}