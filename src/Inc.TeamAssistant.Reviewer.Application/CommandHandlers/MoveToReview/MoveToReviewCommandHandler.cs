using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;
using Inc.TeamAssistant.Users;
using MediatR;
using Telegram.Bot;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview;

internal sealed class MoveToReviewCommandHandler : IRequestHandler<MoveToReviewCommand, MoveToReviewResult>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IPersonRepository _personRepository;
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly TelegramBotClient _client;

    public MoveToReviewCommandHandler(
        ITeamRepository teamRepository,
        IPersonRepository personRepository,
        ITaskForReviewRepository taskForReviewRepository,
        IMessageBuilderService messageBuilderService,
        TelegramBotClient client)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _messageBuilderService =
            messageBuilderService ?? throw new ArgumentNullException(nameof(messageBuilderService));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<MoveToReviewResult> Handle(MoveToReviewCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var currentTeam = await _teamRepository.Find(command.TeamId, cancellationToken);
        if (currentTeam is null)
            throw new ApplicationException($"Team {command.TeamId} was not found.");
            
        var owner = currentTeam.Players.SingleOrDefault(p => p.Id == command.PersonId)
                    ?? await _personRepository.Find(UserIdentity.Create(command.PersonId), cancellationToken);
        if (owner is null)
            throw new ApplicationException($"User {command.PersonFirstName} was not found.");
            
        var lastReviewer = await _personRepository.FindLastReviewer(currentTeam.Id, cancellationToken);
        var targetPlayer = command.TargetUser is { }
            ? await _personRepository.Find(command.TargetUser, cancellationToken)
            : null;
        var reviewer = targetPlayer ?? currentTeam.GetNextReviewer(owner, lastReviewer);
        var taskForReview = new TaskForReview(currentTeam.Id, owner, reviewer, currentTeam.ChatId, command.Description);
        
        var taskForReviewMessage = await _messageBuilderService.NewTaskForReviewBuild(
            command.PersonLanguageId,
            taskForReview);
        var message = await _client.SendTextMessageAsync(
            currentTeam.ChatId,
            taskForReviewMessage.Text,
            entities: taskForReviewMessage.Entities,
            cancellationToken: cancellationToken);
        taskForReview.AttachMessage(message.MessageId);
        
        await _taskForReviewRepository.Upsert(taskForReview, cancellationToken);

        return new MoveToReviewResult();
    }
}