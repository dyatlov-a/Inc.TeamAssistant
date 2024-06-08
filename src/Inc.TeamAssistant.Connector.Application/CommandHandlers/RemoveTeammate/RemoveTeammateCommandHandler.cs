using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.RemoveTeammate;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.RemoveTeammate;

internal sealed class RemoveTeammateCommandHandler : IRequestHandler<RemoveTeammateCommand>
{
    private readonly IPersonRepository _personRepository;

    public RemoveTeammateCommandHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task Handle(RemoveTeammateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.UntilDate.HasValue)
            await _personRepository.LeaveFromTeam(command.TeamId, command.PersonId, command.UntilDate.Value, token);
        else
            await _personRepository.LeaveFromTeam(command.TeamId, command.PersonId, token);
    }
}