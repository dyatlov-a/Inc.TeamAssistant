using System.Text;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.SelectPairs;
using MediatR;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.SelectPairs;

internal sealed class SelectPairsCommandHandler : IRequestHandler<SelectPairsCommand, CommandResult>
{
    private readonly IRandomCoffeeRepository _repository;
    private readonly ITeamAccessor _teamAccessor;
    private readonly RandomCoffeeOptions _options;

    public SelectPairsCommandHandler(
        IRandomCoffeeRepository repository,
        ITeamAccessor teamAccessor,
        RandomCoffeeOptions options)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<CommandResult> Handle(SelectPairsCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var randomCoffeeEntry = await _repository.Find(command.RandomCoffeeEntryId, token);
        if (randomCoffeeEntry is null)
            throw new TeamAssistantException($"RandomCoffeeEntry {command.RandomCoffeeEntryId} was not found.");

        var builder = new StringBuilder();
        randomCoffeeEntry.MoveToNextRound(_options.RoundInterval - _options.WaitingInterval);
        
        if (randomCoffeeEntry.CanSelectPairs())
        {
            var randomCoffeeHistory = randomCoffeeEntry.SelectPairs();

            builder.AppendLine("Пары:");
            
            foreach (var pair in randomCoffeeHistory.Pairs)
            {
                var firstPerson = await _teamAccessor.FindPerson(pair.FirstId, token);
                var secondPerson = await _teamAccessor.FindPerson(pair.SecondId, token);

                if (firstPerson.HasValue && secondPerson.HasValue)
                    builder.AppendLine($"{firstPerson.Value.PersonDisplayName} - {secondPerson.Value.PersonDisplayName}");
            }

            if (randomCoffeeHistory.ExcludedPersonId.HasValue)
            {
                var excludedPerson = await _teamAccessor.FindPerson(randomCoffeeHistory.ExcludedPersonId.Value, token);
            
                if (excludedPerson.HasValue)
                    builder.AppendLine($"К сожалению не найдена пара для: {excludedPerson.Value.PersonDisplayName}");
            }
        }
        else
            builder.AppendLine("Недостаточно участников для составления пар");

        await _repository.Upsert(randomCoffeeEntry, token);
        
        return CommandResult.Build(NotificationMessage.Create(randomCoffeeEntry.ChatId, builder.ToString()));
    }
}