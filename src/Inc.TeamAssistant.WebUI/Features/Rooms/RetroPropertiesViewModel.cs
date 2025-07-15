using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroTemplates;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyTemplates;

namespace Inc.TeamAssistant.WebUI.Features.Rooms;

public sealed record RetroPropertiesViewModel(
    IReadOnlyCollection<RetroTemplateDto> RetroTemplates,
    IReadOnlyCollection<SurveyTemplateDto> SurveyTemplates)
    : IWithEmpty<RetroPropertiesViewModel>
{
    public static RetroPropertiesViewModel Empty { get; } = new(RetroTemplates: [], SurveyTemplates: []);
}