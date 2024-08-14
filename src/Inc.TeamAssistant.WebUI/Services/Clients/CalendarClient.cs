using System.Net.Http.Json;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateCalendar;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;
using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.Clients;

internal sealed class CalendarClient : ICalendarService
{
    private readonly HttpClient _client;

    public CalendarClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }
    
    public async Task<ServiceResult<GetCalendarByOwnerResult?>> GetCalendarByOwner(CancellationToken token)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<ServiceResult<GetCalendarByOwnerResult?>>("calendars", token);
            if (result is null)
                throw new TeamAssistantException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetCalendarByOwnerResult?>(ex.Message);
        }
    }

    public async Task<ServiceResult<Guid>> Create(CreateCalendarCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        try
        {
            var response = await _client.PostAsJsonAsync("calendars", command, token);
        
            response.EnsureSuccessStatusCode();
        
            var result = await response.Content.ReadFromJsonAsync<ServiceResult<Guid>>(token);
            if (result is null)
                throw new TeamAssistantException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<Guid>(ex.Message);
        }
    }

    public async Task<ServiceResult<Guid>> Update(UpdateCalendarCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        try
        {
            var response = await _client.PutAsJsonAsync("calendars", command, token);
        
            response.EnsureSuccessStatusCode();
        
            var result = await response.Content.ReadFromJsonAsync<ServiceResult<Guid>>(token);
            if (result is null)
                throw new TeamAssistantException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<Guid>(ex.Message);
        }
    }
}