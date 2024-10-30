using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.AttachPoll;
using MediatR;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.AttachPoll;

internal sealed class AttachPollCommandHandler : IRequestHandler<AttachPollCommand, CommandResult>
{
    private readonly IRandomCoffeeRepository _repository;

    public AttachPollCommandHandler(IRandomCoffeeRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<CommandResult> Handle(AttachPollCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var randomCoffeeEntry = await _repository.Find(command.RandomCoffeeEntryId, token);
        if (randomCoffeeEntry is null || randomCoffeeEntry.Refused is true)
            throw new TeamAssistantException($"RandomCoffeeEntry {command.RandomCoffeeEntryId} was not found.");

        randomCoffeeEntry.AttachPoll(command.PollId);
        
        await _repository.Upsert(randomCoffeeEntry, token);

        return CommandResult.Empty;
    }
}