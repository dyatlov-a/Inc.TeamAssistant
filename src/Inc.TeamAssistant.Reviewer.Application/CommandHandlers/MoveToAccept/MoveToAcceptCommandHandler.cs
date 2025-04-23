using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
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

    public MoveToAcceptCommandHandler(
        ITaskForReviewRepository repository,
        IReviewMessageBuilder reviewMessageBuilder,
        IReviewMetricsProvider metricsProvider,
        INextReviewerStrategyFactory reviewerFactory,
        ITeamAccessor teamAccessor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
        _metricsProvider = metricsProvider ?? throw new ArgumentNullException(nameof(metricsProvider));
        _reviewerFactory = reviewerFactory ?? throw new ArgumentNullException(nameof(reviewerFactory));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<CommandResult> Handle(MoveToAcceptCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var taskForReview = await command.TaskId.Required(_repository.Find, token);
        if (!taskForReview.CanAccept())
            return CommandResult.Empty;

        var nextReviewerId = await GetNextReviewer(taskForReview, token);

        await _repository.Upsert(
            taskForReview.FinishRound(DateTimeOffset.UtcNow, command.HasComments, nextReviewerId),
            token);

        var notifications = await _reviewMessageBuilder.Build(
            taskForReview,
            command.MessageContext.Bot,
            fromOwner: false,
            token);
        await _metricsProvider.Add(taskForReview, token);
        return CommandResult.Build(notifications.ToArray());
    }

    private async Task<long?> GetNextReviewer(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        
        var finalizes = await _teamAccessor.GetFinalizes(taskForReview.TeamId, DateTimeOffset.UtcNow, token);

        if (!finalizes.Any())
            return null;

        var nextReviewerStrategy = await _reviewerFactory.Create(
            taskForReview.TeamId,
            taskForReview.OwnerId,
            NextReviewerType.SecondRoundRobinForTeam,
            targetPersonId: null,
            finalizes.Select(t => t.Id).ToArray(),
            excludePersonId: taskForReview.ReviewerId,
            token);
        
        var result = nextReviewerStrategy.GetReviewer();
        return result;
    }
}