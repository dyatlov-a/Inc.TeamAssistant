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
    private readonly IMessageBuilder _messageBuilder;

    public SelectPairsCommandHandler(
        IRandomCoffeeRepository repository,
        ITeamAccessor teamAccessor,
        RandomCoffeeOptions options,
        IMessageBuilder messageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(SelectPairsCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var randomCoffeeEntry = await _repository.Find(command.RandomCoffeeEntryId, token);
        if (randomCoffeeEntry is null)
            throw new TeamAssistantException($"RandomCoffeeEntry {command.RandomCoffeeEntryId} was not found.");
        var owner = await _teamAccessor.FindPerson(randomCoffeeEntry.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantException($"Owner {randomCoffeeEntry.OwnerId} was not found.");

        var languageId = owner.GetLanguageId();
        var builder = new StringBuilder();
        randomCoffeeEntry.MoveToNextRound(_options.RoundInterval - _options.WaitingInterval);
        
        if (randomCoffeeEntry.CanSelectPairs())
        {
            var randomCoffeeHistory = randomCoffeeEntry.SelectPairs();

            builder.AppendLine(await _messageBuilder.Build(Messages.RandomCoffee_SelectedPairs, languageId));
            
            foreach (var pair in randomCoffeeHistory.Pairs)
            {
                var firstPerson = await _teamAccessor.FindPerson(pair.FirstId, token);
                var secondPerson = await _teamAccessor.FindPerson(pair.SecondId, token);

                if (firstPerson is not null && secondPerson is not null)
                    builder.AppendLine($"{firstPerson.DisplayName} - {secondPerson.DisplayName}");
            }

            if (randomCoffeeHistory.ExcludedPersonId.HasValue)
            {
                var excludedPerson = await _teamAccessor.FindPerson(randomCoffeeHistory.ExcludedPersonId.Value, token);
            
                if (excludedPerson is not null)
                    builder.AppendLine(await _messageBuilder.Build(
                        Messages.RandomCoffee_NotSelectedPair,
                        languageId,
                        excludedPerson.DisplayName));
            }

            builder.AppendLine(await _messageBuilder.Build(Messages.RandomCoffee_MeetingDescription, languageId));
        }
        else
            builder.AppendLine(await _messageBuilder.Build(Messages.RandomCoffee_NotEnoughParticipants, languageId));

        await _repository.Upsert(randomCoffeeEntry, token);
        
        return CommandResult.Build(NotificationMessage.Create(randomCoffeeEntry.ChatId, builder.ToString()));
    }
}