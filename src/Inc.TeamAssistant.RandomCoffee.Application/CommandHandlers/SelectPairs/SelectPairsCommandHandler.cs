using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.SelectPairs.Services;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.SelectPairs;
using MediatR;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.SelectPairs;

internal sealed class SelectPairsCommandHandler : IRequestHandler<SelectPairsCommand, CommandResult>
{
    private readonly IRandomCoffeeRepository _repository;
    private readonly ITeamAccessor _teamAccessor;
    private readonly RandomCoffeeOptions _options;
    private readonly NotificationsBuilder _notificationsBuilder;

    public SelectPairsCommandHandler(
        IRandomCoffeeRepository repository,
        ITeamAccessor teamAccessor,
        RandomCoffeeOptions options,
        NotificationsBuilder notificationsBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _notificationsBuilder = notificationsBuilder ?? throw new ArgumentNullException(nameof(notificationsBuilder));
    }

    public async Task<CommandResult> Handle(SelectPairsCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var randomCoffeeEntry = await _repository.Find(command.RandomCoffeeEntryId, token);
        if (randomCoffeeEntry is null)
            throw new TeamAssistantException($"RandomCoffeeEntry {command.RandomCoffeeEntryId} was not found.");
        
        var owner = await _teamAccessor.FindPerson(randomCoffeeEntry.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantException($"Owner {randomCoffeeEntry.OwnerId} was not found.");
        
        var languageId = await _teamAccessor.GetClientLanguage(owner.Id, token);
        var randomCoffeeHistory = randomCoffeeEntry.CanSelectPairs()
            ? randomCoffeeEntry.SelectPairs()
            : null;
        var message = await _notificationsBuilder.Build(languageId, randomCoffeeHistory, token);
        
        randomCoffeeEntry.MoveToNextRound(_options.RoundInterval - _options.WaitingInterval);
        
        await _repository.Upsert(randomCoffeeEntry, token);
        
        return CommandResult.Build(NotificationMessage.Create(randomCoffeeEntry.ChatId, message));
    }
}