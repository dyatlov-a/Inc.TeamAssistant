using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;
using Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
using Inc.TeamAssistant.Retro.Model.Queries.GetActionItems;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;
using Inc.TeamAssistant.WebUI.Contracts;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class RetroService : IRetroService
{
    private readonly IMediator _mediator;

    public RetroService(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<GetRetroStateResult> GetRetroState(Guid teamId, CancellationToken token)
    {
        return await _mediator.Send(new GetRetroStateQuery(teamId), token);
    }

    public async Task StartRetro(StartRetroCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, token);
    }

    public async Task MoveToNextRetroState(MoveToNextRetroStateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, token);
    }

    public async Task<GetActionItemsResult> GetActionItems(Guid teamId, CancellationToken token)
    {
        return await _mediator.Send(new GetActionItemsQuery(teamId), token);
    }

    public async Task ChangeActionItem(ChangeActionItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, token);
    }
}