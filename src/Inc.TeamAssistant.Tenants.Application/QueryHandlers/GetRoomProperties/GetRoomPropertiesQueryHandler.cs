using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomProperties;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.QueryHandlers.GetRoomProperties;

internal sealed class GetRoomPropertiesQueryHandler : IRequestHandler<GetRoomPropertiesQuery, GetRoomPropertiesResult>
{
    private readonly IRoomPropertiesProvider _propertiesProvider;
    private readonly ITenantReader _reader;

    public GetRoomPropertiesQueryHandler(IRoomPropertiesProvider propertiesProvider, ITenantReader reader)
    {
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetRoomPropertiesResult> Handle(GetRoomPropertiesQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var roomProperties = await _propertiesProvider.Get(query.RoomId, token);
        var retroTemplates = await _reader.GetRetroTemplates(token);
        var surveyTemplates = await _reader.GetSurveyTemplates(token);

        return new GetRoomPropertiesResult(
            roomProperties.FacilitatorId,
            roomProperties.RetroTemplateId,
            roomProperties.SurveyTemplateId,
            roomProperties.TimerDuration,
            roomProperties.VoteCount,
            roomProperties.VoteByItemCount,
            roomProperties.RetroType,
            retroTemplates,
            surveyTemplates);
    }
}