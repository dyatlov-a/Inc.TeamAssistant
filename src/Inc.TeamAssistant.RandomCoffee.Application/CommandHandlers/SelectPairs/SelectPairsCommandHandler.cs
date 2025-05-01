using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.SelectPairs.Services;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.SelectPairs;
using MediatR;
using Inc.TeamAssistant.Primitives.Extensions;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.SelectPairs;

internal sealed class SelectPairsCommandHandler : IRequestHandler<SelectPairsCommand, CommandResult>
{
    private readonly IRandomCoffeeRepository _repository;
    private readonly ITeamAccessor _teamAccessor;
    private readonly SelectPairsNotificationBuilder _notificationBuilder;
    private readonly IMessageBuilder _messageBuilder;

    public SelectPairsCommandHandler(
        IRandomCoffeeRepository repository,
        ITeamAccessor teamAccessor,
        SelectPairsNotificationBuilder notificationBuilder,
        IMessageBuilder messageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _notificationBuilder = notificationBuilder ?? throw new ArgumentNullException(nameof(notificationBuilder));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(SelectPairsCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var botContext = command.MessageContext.Bot;
        
        var entry = await command.RandomCoffeeEntryId.Required(_repository.Find, token);
        if (entry.AlreadyStarted(onDemand: false))
            return CommandResult.Empty;

        var pairs = entry.TrySelectPairs();
        var pollMessageId = entry.MoveToNextRound(
            DateTimeOffset.UtcNow,
            botContext.GetRoundInterval(),
            botContext.GetVotingInterval());

        await _repository.Upsert(entry, token);
        
        var languageId = await _teamAccessor.GetClientLanguage(
            command.MessageContext.Bot.Id,
            entry.OwnerId,
            token);
        var notEnoughMessage = _messageBuilder.Build(Messages.RandomCoffee_NotEnoughParticipants, languageId);
        var notifications = new List<NotificationMessage>
        {
            pairs is not null
                ? await _notificationBuilder.Build(
                    entry.ChatId,
                    entry.BotId,
                    languageId,
                    pairs,
                    token)
                : NotificationMessage.Create(entry.ChatId, notEnoughMessage)
        };
        
        if (pollMessageId.HasValue)
            notifications.Add(NotificationMessage.EndPoll(entry.ChatId, pollMessageId.Value));
        
        return CommandResult.Build(notifications.ToArray());
    }
}