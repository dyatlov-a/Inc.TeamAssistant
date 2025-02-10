using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.SelectPairs.Services;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.SelectPairs;
using MediatR;

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

        var randomCoffeeEntry = await _repository.Find(command.RandomCoffeeEntryId, token);
        if (randomCoffeeEntry is null)
            throw new TeamAssistantException($"RandomCoffeeEntry {command.RandomCoffeeEntryId} was not found.");
        
        if (randomCoffeeEntry.Refused)
            return CommandResult.Empty;
        
        var botContext = await _botAccessor.GetBotContext(randomCoffeeEntry.BotId, token);
        var owner = await _teamAccessor.FindPerson(randomCoffeeEntry.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantException($"Owner {randomCoffeeEntry.OwnerId} was not found.");
        
        var languageId = await _teamAccessor.GetClientLanguage(command.MessageContext.Bot.Id, owner.Id, token);
        var notificationMessage = randomCoffeeEntry.CanSelectPairs()
            ? await _notificationsBuilder.Build(
                randomCoffeeEntry.ChatId,
                randomCoffeeEntry.BotId,
                languageId,
                randomCoffeeEntry.SelectPairs(),
                token)
            : NotificationMessage.Create(
                randomCoffeeEntry.ChatId,
                await _messageBuilder.Build(Messages.RandomCoffee_NotEnoughParticipants, languageId));
        
        randomCoffeeEntry.MoveToNextRound(
            DateTimeOffset.UtcNow,
            botContext.GetRoundInterval(),
            botContext.GetVotingInterval());
        
        await _repository.Upsert(randomCoffeeEntry, token);
        
        return CommandResult.Build(notificationMessage);
    }
}