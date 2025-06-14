using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Application.Common.Converters;

internal static class RetroPropertiesConverter
{
    public static RetroPropertiesDto ConvertTo(RetroProperties properties)
    {
        ArgumentNullException.ThrowIfNull(properties);
        
        var templateId = properties.TemplateId ?? Guid.Parse("41c7a7b9-044f-46aa-b94e-e3bb06aed70c");
        var timerDuration = properties.TimerDuration ?? TimeSpan.FromMinutes(10);
        var voteCount = properties.VoteCount ?? 5;

        return new RetroPropertiesDto(properties.FacilitatorId, templateId, timerDuration, voteCount);
    }
}