namespace Inc.TeamAssistant.Survey.Domain;

public sealed class Question
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = default!;
    public string Text { get; private set; } = default!;

    private Question()
    {
    }

    public Question(Guid id, string title, string text)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        
        Id = id;
        Title = title;
        Text = text;
    }
}