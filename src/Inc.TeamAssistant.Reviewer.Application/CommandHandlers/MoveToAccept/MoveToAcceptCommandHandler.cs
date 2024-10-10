using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept;

internal sealed class MoveToAcceptCommandHandler : IRequestHandler<MoveToAcceptCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;
    private readonly ITeamAccessor _teamAccessor;
    private readonly IReviewMetricsProvider _reviewMetricsProvider;

    public MoveToAcceptCommandHandler(
        ITaskForReviewRepository taskForReviewRepository,
        IReviewMessageBuilder reviewMessageBuilder,
        ITeamAccessor teamAccessor,
        IReviewMetricsProvider reviewMetricsProvider)
    {
        _taskForReviewRepository = taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _reviewMetricsProvider = reviewMetricsProvider ?? throw new ArgumentNullException(nameof(reviewMetricsProvider));
    }

    public async Task<CommandResult> Handle(MoveToAcceptCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var taskForReview = await _taskForReviewRepository.GetById(command.TaskId, token);
        if (!taskForReview.CanAccept())
            return CommandResult.Empty;

        var reviewer = await _teamAccessor.FindPerson(taskForReview.ReviewerId, token);
        if (reviewer is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.ReviewerId);
        
        var owner = await _teamAccessor.FindPerson(taskForReview.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.OwnerId);

        taskForReview.Accept(DateTimeOffset.UtcNow, command.AcceptedWithComments);

        var notifications = await _reviewMessageBuilder.Build(
            command.MessageContext.ChatMessage.MessageId,
            taskForReview,
            reviewer,
            owner,
            command.MessageContext.Bot,
            token);

        await _taskForReviewRepository.Upsert(taskForReview, token);

        await _reviewMetricsProvider.Add(taskForReview, token);

        return CommandResult.Build(notifications.ToArray());
    }
}