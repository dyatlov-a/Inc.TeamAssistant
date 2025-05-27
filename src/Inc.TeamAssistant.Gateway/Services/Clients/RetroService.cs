using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;
using Inc.TeamAssistant.Retro.Model.Commands.RemoveRetroItem;
using Inc.TeamAssistant.Retro.Model.Commands.SetVotes;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
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

    public async Task<CreateRetroItemResult> CreateRetroItem(CreateRetroItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        return await _mediator.Send(command, token);
    }

    public async Task UpdateRetroItem(UpdateRetroItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _mediator.Send(command, token);
    }

    public async Task RemoveRetroItem(Guid retroItemId, CancellationToken token)
    {
        await _mediator.Send(new RemoveRetroItemCommand(retroItemId), token);
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

    public async Task SetVotes(SetVotesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, token);
    }
}