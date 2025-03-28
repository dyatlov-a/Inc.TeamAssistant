using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept;

internal sealed class MoveToAcceptCommandHandler : IRequestHandler<MoveToAcceptCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _repository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;
    private readonly ITeamAccessor _teamAccessor;
    private readonly IReviewMetricsProvider _metricsProvider;

    public MoveToAcceptCommandHandler(
        ITaskForReviewRepository repository,
        IReviewMessageBuilder reviewMessageBuilder,
        ITeamAccessor teamAccessor,
        IReviewMetricsProvider metricsProvider)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _metricsProvider = metricsProvider ?? throw new ArgumentNullException(nameof(metricsProvider));
    }

    public async Task<CommandResult> Handle(MoveToAcceptCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var taskForReview = await _repository.GetById(command.TaskId, token);
        if (!taskForReview.CanAccept())
            return CommandResult.Empty;

        var reviewer = await _teamAccessor.EnsurePerson(taskForReview.ReviewerId, token);
        var owner = await _teamAccessor.EnsurePerson(taskForReview.OwnerId, token);

        await _repository.Upsert(taskForReview.Accept(DateTimeOffset.UtcNow, command.HasComments), token);

        var notifications = await _reviewMessageBuilder.Build(
            command.MessageContext.ChatMessage.MessageId,
            taskForReview,
            reviewer,
            owner,
            command.MessageContext.Bot,
            token);
        await _metricsProvider.Add(taskForReview, token);
        return CommandResult.Build(notifications.ToArray());
    }
}