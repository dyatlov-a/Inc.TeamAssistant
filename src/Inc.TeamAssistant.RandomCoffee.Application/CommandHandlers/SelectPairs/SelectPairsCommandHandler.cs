using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
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
    private readonly NotificationsBuilder _notificationsBuilder;
    private readonly IMessageBuilder _messageBuilder;
    private readonly IBotAccessor _botAccessor;

    public SelectPairsCommandHandler(
        IRandomCoffeeRepository repository,
        ITeamAccessor teamAccessor,
        NotificationsBuilder notificationsBuilder,
        IMessageBuilder messageBuilder,
        IBotAccessor botAccessor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _notificationsBuilder = notificationsBuilder ?? throw new ArgumentNullException(nameof(notificationsBuilder));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
    }

    public async Task<CommandResult> Handle(SelectPairsCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var entry = await command.RandomCoffeeEntryId.Required(_repository.Find, token);
        if (entry.AlreadyStarted(onDemand: false))
            return CommandResult.Empty;
        
        var botContext = await _botAccessor.GetBotContext(entry.BotId, token);
        var languageId = await _teamAccessor.GetClientLanguage(
            command.MessageContext.Bot.Id,
            entry.OwnerId,
            token);
        var notificationMessage = entry.CanSelectPairs()
            ? await _notificationsBuilder.Build(
                entry.ChatId,
                entry.BotId,
                languageId,
                entry.SelectPairs(),
                token)
            : NotificationMessage.Create(
                entry.ChatId,
                await _messageBuilder.Build(Messages.RandomCoffee_NotEnoughParticipants, languageId));

        await _repository.Upsert(
            entry.MoveToNextRound(DateTimeOffset.UtcNow, botContext.GetRoundInterval(), botContext.GetVotingInterval()),
            token);
        
        return CommandResult.Build(notificationMessage);
    }
}