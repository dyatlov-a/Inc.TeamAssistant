using Inc.TeamAssistant.Tenants.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateTeam;
using Inc.TeamAssistant.Tenants.Model.Queries.Common;

namespace Inc.TeamAssistant.WebUI.Features.Tenants;

public sealed class TenantTeamFormModel
{
    public string Name { get; set; } = string.Empty;

    public TenantTeamFormModel Apply(TenantTeamDto team)
    {
        ArgumentNullException.ThrowIfNull(team);
        
        Name = team.Name;

        return this;
    }

    public TenantTeamFormModel Clear()
    {
        Name = string.Empty;
        
        return this;
    }

    public CreateTeamCommand ToCommand() => new(Name);
    public UpdateTeamCommand ToCommand(Guid teamId) => new(teamId, Name);
}