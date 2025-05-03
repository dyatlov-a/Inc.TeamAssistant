using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Application.Services;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.RepeatMeeting;
using MediatR;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.RepeatMeeting;

internal sealed class RepeatMeetingCommandHandler : IRequestHandler<RepeatMeetingCommand, CommandResult>
{
    private readonly IRandomCoffeeRepository _repository;
    private readonly PollBuilder _pollBuilder;

    public RepeatMeetingCommandHandler(IRandomCoffeeRepository repository, PollBuilder pollBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pollBuilder = pollBuilder ?? throw new ArgumentNullException(nameof(pollBuilder));
    }
    
    public async Task<CommandResult> Handle(RepeatMeetingCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var entry = await command.MessageContext.ChatMessage.ChatId.Required(_repository.Find, token);
        
        entry.MoveToWaiting(
            DateTimeOffset.UtcNow,
            command.MessageContext.Bot.GetVotingInterval());

        await _repository.Upsert(entry, token);

        var pollNotification = await _pollBuilder.Build(entry, token);
        
        return CommandResult.Build(pollNotification);
    }
}