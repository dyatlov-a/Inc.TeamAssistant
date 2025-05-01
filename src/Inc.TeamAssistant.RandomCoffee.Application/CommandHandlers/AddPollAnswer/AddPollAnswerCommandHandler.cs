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
        
        var entry = await _repository.Find(command.PollId, token);
        if (entry?.IsWaitAnswer() == true)
            await _repository.Upsert(entry.SetAnswer(command.IsAttend, command.MessageContext.Person.Id), token);
        
        return CommandResult.Empty;
    }
}