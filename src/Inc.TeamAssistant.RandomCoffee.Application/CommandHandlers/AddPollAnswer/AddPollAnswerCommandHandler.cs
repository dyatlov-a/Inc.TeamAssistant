using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.AddPollAnswer;
using MediatR;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.AddPollAnswer;

internal sealed class AddPollAnswerCommandHandler : IRequestHandler<AddPollAnswerCommand, CommandResult>
{
    private readonly IRandomCoffeeRepository _repository;

    public AddPollAnswerCommandHandler(IRandomCoffeeRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<CommandResult> Handle(AddPollAnswerCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var randomCoffeeEntry = await _repository.Find(command.PollId, token);
        
        if (randomCoffeeEntry is not null && command.Options.Contains("0"))
        {
            randomCoffeeEntry.AddPerson(command.MessageContext.PersonId);

            await _repository.Upsert(randomCoffeeEntry, token);
        }
        
        return CommandResult.Empty;
    }
}