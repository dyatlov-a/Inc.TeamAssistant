using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;
using MediatR;
using Telegram.Bot;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview;

internal sealed class MoveToReviewCommandHandler : IRequestHandler<MoveToReviewCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly TelegramBotClientProvider _telegramBotClientProvider;
    private readonly ITeamAccessor _teamAccessor;

    public MoveToReviewCommandHandler(
        ITaskForReviewRepository taskForReviewRepository,
        IMessageBuilderService messageBuilderService,
        TelegramBotClientProvider telegramBotClientProvider,
        ITeamAccessor teamAccessor)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _messageBuilderService =
            messageBuilderService ?? throw new ArgumentNullException(nameof(messageBuilderService));
        _telegramBotClientProvider = telegramBotClientProvider ?? throw new ArgumentNullException(nameof(telegramBotClientProvider));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<CommandResult> Handle(MoveToReviewCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var client = _telegramBotClientProvider.Get();
        var teammates = await _teamAccessor.GetTeammates(command.TeamId, token);
        var targetTeam = command.MessageContext.FindTeam(command.TeamId);
        if (targetTeam is null)
            throw new ApplicationException($"Team {command.TeamId} was not found.");
        
        var lastReviewerId = await _taskForReviewRepository.FindLastReviewer(command.TeamId, token);
        var targetPlayer = command.MessageContext.TargetUser?.UserId.HasValue == true
            ? (await _teamAccessor.FindPerson(command.MessageContext.TargetUser.UserId.Value, token))?.Id
            : !string.IsNullOrWhiteSpace(command.MessageContext.TargetUser?.Username)
                ? (await _teamAccessor.FindPerson(command.MessageContext.TargetUser.Username, token))?.Id
                : null;
        var reviewer = targetPlayer ?? new RoundRobinReviewerStrategy(teammates.Select(t => t.PersonId).ToArray())
            .Next(command.MessageContext.PersonId, lastReviewerId);
        var taskForReview = new TaskForReview(
            command.TeamId,
            command.MessageContext.PersonId,
            reviewer,
            targetTeam.ChatId,
            command.Description);
        
        var taskForReviewMessage = await _messageBuilderService.NewTaskForReviewBuild(
            command.MessageContext.LanguageId,
            taskForReview,
            token);
        var message = await client.SendTextMessageAsync(
            targetTeam.ChatId,
            taskForReviewMessage.Text,
            entities: taskForReviewMessage.Entities,
            cancellationToken: token);
        taskForReview.AttachMessage(message.MessageId);
        
        await _taskForReviewRepository.Upsert(taskForReview, token);

        return CommandResult.Empty;
    }
}