using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
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

        var entry = await command.RandomCoffeeEntryId.Required(_repository.Find, token);
        
        await _repository.Upsert(entry.AttachPoll(command.PollId, command.MessageId), token);

        return CommandResult.Empty;
    }
}