using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
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
    
    public async Task<CreateRetroItemResult> CreateRetroItem(CreateRetroItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        return await _mediator.Send(command, token);
    }
}