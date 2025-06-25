namespace Inc.TeamAssistant.Survey.Domain;

public sealed class SurveyTemplate
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public IReadOnlyCollection<Guid> QuestionIds { get; private set; } = default!;
}