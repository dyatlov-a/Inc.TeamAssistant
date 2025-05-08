using Inc.TeamAssistant.Tenants.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateTeam;
using Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableTeams;
using Inc.TeamAssistant.Tenants.Model.Queries.GetTeam;
using Inc.TeamAssistant.WebUI.Contracts;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class TenantService : ITenantService
{
    private readonly IMediator _mediator;

    public TenantService(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<GetAvailableTeamsResult> GetAvailableTeams(Guid? id, CancellationToken token)
    {
        return await _mediator.Send(new GetAvailableTeamsQuery(id), token);
    }

    public async Task<GetTeamResult> GetTeam(
        Guid id,
        CancellationToken token)
    {
        return await _mediator.Send(new GetTeamQuery(id), token);
    }

    public async Task<CreateTeamResult> CreateTeam(
        CreateTeamCommand command,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        return await _mediator.Send(command, token);
    }

    public async Task UpdateTeam(
        UpdateTeamCommand command,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, token);
    }
}