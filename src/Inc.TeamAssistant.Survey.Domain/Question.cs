namespace Inc.TeamAssistant.Survey.Domain;

public sealed class Question
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = default!;
    public string Text { get; private set; } = default!;
}