using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using Inc.TeamAssistant.Retro.Model.Commands.SetVotes;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using Inc.TeamAssistant.WebUI;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Hubs;

internal sealed class RetroHub : Hub<IRetroHubClient>
{
    private readonly IRetroService _retroService;

    public RetroHub(IRetroService retroService)
    {
        _retroService = retroService ?? throw new ArgumentNullException(nameof(retroService));
    }

    [HubMethodName(HubDescriptors.RetroHub.JoinToRetroMethod)]
    public async Task JoinToRetro(Guid groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString("N"));
    }
    
    [HubMethodName(HubDescriptors.RetroHub.RemoveFromRetroMethod)]
    public async Task RemoveFromRetro(Guid groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString("N"));
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
}