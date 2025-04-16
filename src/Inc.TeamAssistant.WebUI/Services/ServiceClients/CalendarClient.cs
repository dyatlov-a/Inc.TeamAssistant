using System.Net.Http.Json;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;
using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class CalendarClient : ICalendarService
{
    private readonly HttpClient _client;

    public CalendarClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }
    
    public async Task<GetCalendarByOwnerResult> GetCalendarByOwner(CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetCalendarByOwnerResult>("calendars", token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<UpdateCalendarResult> Update(UpdateCalendarCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PutAsJsonAsync("calendars", command, token);
        
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<UpdateCalendarResult>(token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }
}