using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using Inc.TeamAssistant.Retro.Model.Commands.SetVotes;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using Inc.TeamAssistant.WebUI;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Hubs;

[Authorize]
internal sealed class RetroHub : Hub<IRetroHubClient>
{
    private readonly IRetroService _retroService;
    private readonly OnlinePersonService _personService;

    public RetroHub(IRetroService retroService, OnlinePersonService personService)
    {
        _retroService = retroService ?? throw new ArgumentNullException(nameof(retroService));
        _personService = personService ?? throw new ArgumentNullException(nameof(personService));
    }

    [HubMethodName(HubDescriptors.RetroHub.JoinRetroMethod)]
    public async Task JoinRetro(Guid groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString("N"));
        
        await _personService.AddToTeam(Context.ConnectionId, groupId);
    }
    
    [HubMethodName(HubDescriptors.RetroHub.LeaveRetroMethod)]
    public async Task LeaveRetro(Guid groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString("N"));
        
        await _personService.LeaveFromTeam(Context.ConnectionId, groupId);
    }

    [HubMethodName(HubDescriptors.RetroHub.CreateRetroItemMethod)]
    public async Task CreateRetroItem(CreateRetroItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _retroService.CreateRetroItem(command);
    }

    [HubMethodName(HubDescriptors.RetroHub.UpdateRetroItemMethod)]
    public async Task UpdateRetroItem(UpdateRetroItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _retroService.UpdateRetroItem(command);
    }
    
    [HubMethodName(HubDescriptors.RetroHub.RemoveRetroItemMethod)]
    public async Task RemoveRetroItem(Guid retroItemId)
    {
        await _retroService.RemoveRetroItem(retroItemId);
    }
    
    [HubMethodName(HubDescriptors.RetroHub.SetVotesMethod)]
    public async Task SetVotes(SetVotesCommand command)
    {
        await _retroService.SetVotes(command);
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var teamIds = await _personService.LeaveFromAllTeams(Context.ConnectionId);

        foreach (var teamId in teamIds)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamId.ToString("N"));

        await base.OnDisconnectedAsync(exception);
    }
}