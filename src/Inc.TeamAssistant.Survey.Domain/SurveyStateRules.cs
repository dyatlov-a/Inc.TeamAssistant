namespace Inc.TeamAssistant.Survey.Domain;

public static class SurveyStateRules
{
    public static readonly IReadOnlyCollection<SurveyState> Active =
    [
        SurveyState.InProgress
    ];
}