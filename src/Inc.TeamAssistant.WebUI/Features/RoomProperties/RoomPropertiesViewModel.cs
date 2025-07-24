using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroTemplates;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyTemplates;

namespace Inc.TeamAssistant.WebUI.Features.RoomProperties;

public sealed record RoomPropertiesViewModel(
    IReadOnlyCollection<RetroTemplateDto> RetroTemplates,
    IReadOnlyCollection<SurveyTemplateDto> SurveyTemplates)
    : IWithEmpty<RoomPropertiesViewModel>
{
    public static RoomPropertiesViewModel Empty { get; } = new(RetroTemplates: [], SurveyTemplates: []);
}