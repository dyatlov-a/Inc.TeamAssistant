namespace Inc.TeamAssistant.Survey.Domain;

public sealed class SurveyTemplate
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public IReadOnlyCollection<Guid> QuestionIds { get; private set; } = default!;

    private SurveyTemplate()
    {
    }

    public SurveyTemplate(Guid id, string name, IReadOnlyCollection<Guid> questionIds)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Id = id;
        Name = name;
        QuestionIds = questionIds ?? throw new ArgumentNullException(nameof(questionIds));
    }
}