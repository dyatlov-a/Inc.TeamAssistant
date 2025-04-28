using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Teams;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.RemoveTeammate;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.RemoveTeammate;

internal sealed class RemoveTeammateCommandHandler : IRequestHandler<RemoveTeammateCommand>
{
    private readonly ITeammateRepository _repository;
    private readonly ICurrentPersonResolver _personProvider;
    private readonly ITeamAccessor _teamAccessor;
    private readonly ILeaveTeamHandler _leaveTeamHandler;

    public RemoveTeammateCommandHandler(
        ITeammateRepository repository,
        ICurrentPersonResolver personProvider,
        ITeamAccessor teamAccessor,
        ILeaveTeamHandler leaveTeamHandler)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personProvider = personProvider ?? throw new ArgumentNullException(nameof(personProvider));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _leaveTeamHandler = leaveTeamHandler ?? throw new ArgumentNullException(nameof(leaveTeamHandler));
    }

    public async Task Handle(RemoveTeammateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var teammateKey = new TeammateKey(command.TeamId, command.PersonId);
        var currentPerson = _personProvider.GetCurrentPerson();
        
        await _teamAccessor.EnsureManagerAccess(new(command.TeamId, currentPerson.Id), token);
        
        await _repository.RemoveFromTeam(teammateKey, token);

        await _leaveTeamHandler.Handle(teammateKey, token);
    }
}