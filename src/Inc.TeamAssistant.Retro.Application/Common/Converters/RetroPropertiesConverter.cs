using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Application.Common.Converters;

internal static class RetroPropertiesConverter
{
    private static readonly Guid DefaultRetroTemplateId = Guid.Parse("41c7a7b9-044f-46aa-b94e-e3bb06aed70c");
    private static readonly Guid DefaultSurveyTemplateId = Guid.Parse("6c9b2eef-b7ce-4e13-b866-1a0cd743c6b3");
    private static readonly TimeSpan DefaultTimerDuration = TimeSpan.FromMinutes(10);
    private static readonly int DefaultVoteCount = 5;
    private static readonly int DefaultVoteByItemCount = 2;
    
    public static RetroPropertiesDto ConvertTo(RetroProperties properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        return new RetroPropertiesDto(
            properties.FacilitatorId,
            properties.RetroTemplateId ?? DefaultRetroTemplateId,
            properties.SurveyTemplateId ?? DefaultSurveyTemplateId,
            properties.TimerDuration ?? DefaultTimerDuration,
            properties.VoteCount ?? DefaultVoteCount,
            properties.VoteByItemCount ?? DefaultVoteByItemCount,
            properties.RequiredRetroType().ToString());
    }
}