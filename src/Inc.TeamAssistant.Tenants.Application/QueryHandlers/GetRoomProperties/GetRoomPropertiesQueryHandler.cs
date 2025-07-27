using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomProperties;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.QueryHandlers.GetRoomProperties;

internal sealed class GetRoomPropertiesQueryHandler : IRequestHandler<GetRoomPropertiesQuery, GetRoomPropertiesResult>
{
    private readonly ITenantRepository _repository;
    private readonly ITenantReader _reader;

    public GetRoomPropertiesQueryHandler(ITenantRepository repository, ITenantReader reader)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetRoomPropertiesResult> Handle(GetRoomPropertiesQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var room = await query.RoomId.Required(_repository.FindRoom, token);
        var retroTemplates = await _reader.GetRetroTemplates(token);
        var surveyTemplates = await _reader.GetSurveyTemplates(token);

        return new GetRoomPropertiesResult(
            room.Properties.FacilitatorId,
            room.Properties.RetroTemplateId,
            room.Properties.SurveyTemplateId,
            room.Properties.TimerDuration,
            room.Properties.VoteCount,
            room.Properties.VoteByItemCount,
            room.Properties.RetroType,
            retroTemplates,
            surveyTemplates);
    }
}