using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Hubs;

internal sealed class SurveyEventSender : ISurveyEventSender
{
    private readonly IHubContext<SurveyHub, ISurveyHubClient> _hubContext;

    public SurveyEventSender(IHubContext<SurveyHub, ISurveyHubClient> hubContext)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    }

    public async Task SurveyStarted(Guid roomId)
    {
        var surveyRoomId = RoomId.CreateForSurvey(roomId);
        
        await _hubContext.Clients.Group(surveyRoomId.GroupName).SurveyStarted();
    }

    public async Task SurveyStateChanged(Guid roomId, long personId, bool finished)
    {
        var surveyRoomId = RoomId.CreateForSurvey(roomId);
        
        await _hubContext.Clients.Group(surveyRoomId.GroupName).SurveyStateChanged(personId, finished);
    }

    public async Task SurveyFinished(Guid roomId)
    {
        var surveyRoomId = RoomId.CreateForSurvey(roomId);
        
        await _hubContext.Clients.Group(surveyRoomId.GroupName).SurveyFinished();
    }
}