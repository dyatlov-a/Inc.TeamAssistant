using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Primitives.Features.Teams;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.UpdateTeammate;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.UpdateTeammate;

internal sealed class UpdateTeammateCommandHandler : IRequestHandler<UpdateTeammateCommand>
{
    private readonly ITeammateRepository _repository;
    private readonly IPersonResolver _personProvider;
    private readonly ITeamAccessor _teamAccessor;
    private readonly ILeaveTeamHandler _leaveTeamHandler;

    public UpdateTeammateCommandHandler(
        ITeammateRepository repository,
        IPersonResolver personProvider,
        ITeamAccessor teamAccessor, ILeaveTeamHandler leaveTeamHandler)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personProvider = personProvider ?? throw new ArgumentNullException(nameof(personProvider));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _leaveTeamHandler = leaveTeamHandler ?? throw new ArgumentNullException(nameof(leaveTeamHandler));
    }

    public async Task Handle(UpdateTeammateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var teammateKey = new TeammateKey(command.TeamId, command.PersonId);
        var currentPerson = _personProvider.GetCurrentPerson();
        
        await _teamAccessor.EnsureManagerAccess(new(command.TeamId, currentPerson.Id), token);
        
        var teammate = await teammateKey.Required(_repository.Find, token);

        if (command.HasLeaveUntil)
        {
            teammate.ChangeLeaveUntil(command.LeaveUntil);
            
            if (command.LeaveUntil.HasValue)
                await _leaveTeamHandler.Handle(teammateKey, token);
        }
            
        if (command.CanFinalize.HasValue)
            teammate.ChangeCanFinalize(command.CanFinalize.Value);
        
        await _repository.Upsert(teammate, token);
    }
}