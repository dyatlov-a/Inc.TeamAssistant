using Inc.TeamAssistant.Tenants.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateTeam;
using Inc.TeamAssistant.Tenants.Model.Queries.GetTeam;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRetroService
{
    Task<GetTeamResult> GetTeam(
        Guid id,
        CancellationToken token = default);
    
    Task<CreateTeamResult> CreateTeam(
        CreateTeamCommand command,
        CancellationToken token = default);
    
    Task UpdateTeam(
        UpdateTeamCommand command,
        CancellationToken token = default);
}