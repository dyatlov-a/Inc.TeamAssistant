using Inc.TeamAssistant.Holidays.Extensions;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview;

internal sealed class MoveToReviewCommandHandler : IRequestHandler<MoveToReviewCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;
    private readonly ITeamAccessor _teamAccessor;
    private readonly ReviewerOptions _options;
    private readonly ITaskForReviewReader _taskForReviewReader;
    private readonly IDraftTaskForReviewRepository _draftTaskForReviewRepository;
    private readonly DraftTaskForReviewService _draftTaskForReviewService;

    public MoveToReviewCommandHandler(
        ITaskForReviewRepository taskForReviewRepository,
        IReviewMessageBuilder reviewMessageBuilder,
        ITeamAccessor teamAccessor,
        ReviewerOptions options,
        ITaskForReviewReader taskForReviewReader,
        IDraftTaskForReviewRepository draftTaskForReviewRepository,
        DraftTaskForReviewService draftTaskForReviewService)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _reviewMessageBuilder =
            reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
        _draftTaskForReviewRepository =
            draftTaskForReviewRepository ?? throw new ArgumentNullException(nameof(draftTaskForReviewRepository));
        _draftTaskForReviewService =
            draftTaskForReviewService ?? throw new ArgumentNullException(nameof(draftTaskForReviewService));
    }

    public async Task<CommandResult> Handle(MoveToReviewCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var draft = await _draftTaskForReviewRepository.GetById(command.DraftId, token);
        var targetTeam = command.MessageContext.FindTeam(draft.TeamId);
        if (targetTeam is null)
            throw new TeamAssistantUserException(Messages.Connector_TeamNotFound, draft.TeamId);
        
        var teammates = await _teamAccessor.GetTeammates(draft.TeamId, DateTimeOffset.UtcNow, token);
        if (!teammates.Any())
            throw new TeamAssistantUserException(Messages.Reviewer_TeamWithoutUsers, draft.TeamId);

        var ownerId = command.MessageContext.Person.Id;
        var taskForReview = new TaskForReview(
            Guid.NewGuid(),
            draft,
            command.MessageContext.Bot.Id,
            DateTimeOffset.UtcNow,
            _options.NotificationInterval,
            ownerId,
            targetTeam.ChatId);

        if (command.MessageContext.TargetPersonId.HasValue)
            taskForReview.SetConcreteReviewer(command.MessageContext.TargetPersonId.Value);
        else
        {
            var lastReviewerId = await _taskForReviewRepository.FindLastReviewer(draft.TeamId, ownerId, token);
            var history = await _taskForReviewReader.GetHistory(
                taskForReview.TeamId,
                DateTimeOffset.UtcNow.GetLastDayOfWeek(DayOfWeek.Monday),
                token);
            
            taskForReview.DetectReviewer(teammates.Select(t => t.Id).ToArray(), history, lastReviewerId);
        }
        
        var reviewer = await _teamAccessor.FindPerson(taskForReview.ReviewerId, token);
        if (reviewer is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.ReviewerId);
        
        var owner = await _teamAccessor.FindPerson(taskForReview.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.OwnerId);

        var notifications = new List<NotificationMessage>();
        notifications.AddRange(await _reviewMessageBuilder.Build(
            command.MessageContext.ChatMessage.MessageId,
            taskForReview,
            reviewer,
            owner,
            token));
        
        await _taskForReviewRepository.Upsert(taskForReview, token);

        notifications.AddRange(await _draftTaskForReviewService.Delete(draft, token));
        
        return CommandResult.Build(notifications.ToArray());
    }
}