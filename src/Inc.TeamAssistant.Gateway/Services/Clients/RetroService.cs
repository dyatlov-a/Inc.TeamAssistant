using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;
using Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroAssessment;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
using Inc.TeamAssistant.Retro.Model.Queries.GetActionItems;
using Inc.TeamAssistant.Retro.Model.Queries.GetActionItemsHistory;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroAssessment;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroTemplates;
using Inc.TeamAssistant.Tenants.Model.Commands.ChangeRoomProperties;
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

    public async Task<GetRetroStateResult> GetRetroState(Guid roomId, CancellationToken token)
    {
        return await _mediator.Send(new GetRetroStateQuery(roomId), token);
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

    public async Task<GetActionItemsResult> GetActionItems(Guid roomId, int limit, CancellationToken token)
    {
        return await _mediator.Send(new GetActionItemsQuery(roomId, limit), token);
    }

    public async Task<GetActionItemsHistoryResult> GetActionItemsHistory(
        Guid roomId,
        string state,
        int offset,
        int limit,
        CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(state);
        
        return await _mediator.Send(new GetActionItemsHistoryQuery(roomId, state, offset, limit), token);
    }

    public async Task ChangeActionItem(ChangeActionItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, token);
    }

    public async Task<GetRetroAssessmentResult> GetRetroAssessment(Guid sessionId, CancellationToken token)
    {
        return await _mediator.Send(new GetRetroAssessmentQuery(sessionId), token);
    }

    public async Task SetRetroAssessment(SetRetroAssessmentCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, token);
    }

    public async Task<GetRetroTemplatesResult> GetRetroTemplates(CancellationToken token)
    {
        return await _mediator.Send(new GetRetroTemplatesQuery(), token);
    }

    public async Task ChangeRetroProperties(ChangeRoomPropertiesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, token);
    }
}