using Inc.TeamAssistant.Primitives.Commands;
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
        ArgumentNullException.ThrowIfNull(command);
        
        var randomCoffeeEntry = await _repository.Find(command.PollId, token);
        if (randomCoffeeEntry?.Refused is false)
        {
            const string optionYes = "0";
            
            if (command.Options.Contains(optionYes))
                randomCoffeeEntry.AddPerson(command.MessageContext.Person.Id);
            else
                randomCoffeeEntry.RemovePerson(command.MessageContext.Person.Id);

            await _repository.Upsert(randomCoffeeEntry, token);
        }
        
        return CommandResult.Empty;
    }
}