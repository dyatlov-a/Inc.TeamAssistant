using System.Net;
using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.WebUI.Services.Internal;

namespace Inc.TeamAssistant.WebUI.Extensions;

internal static class HttpResponseMessageExtensions
{
    public static async Task HandleValidation(this HttpResponseMessage response, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(response);
        
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var errorDetails = await response.Content.ReadFromJsonAsync<ErrorDetails>(token);
            if (errorDetails is not null)
                throw new ClientException(errorDetails);
        }
        
        response.EnsureSuccessStatusCode();
    }
}