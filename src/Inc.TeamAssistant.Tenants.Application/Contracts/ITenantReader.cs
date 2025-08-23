using Inc.TeamAssistant.Tenants.Domain;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomHistory;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomProperties;

namespace Inc.TeamAssistant.Tenants.Application.Contracts;

public interface ITenantReader
{
    Task<IReadOnlyCollection<Room>> GetAvailableRooms(Guid? teamId, long personId, CancellationToken token);
    
    Task<IReadOnlyCollection<TemplateDto>> GetRetroTemplates(CancellationToken token);
    
    Task<IReadOnlyCollection<TemplateDto>> GetSurveyTemplates(CancellationToken token);

    Task<IReadOnlyCollection<RoomEntryDto>> GetRoomHistory(Guid roomId, DateTimeOffset from, CancellationToken token);
}