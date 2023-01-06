using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed class Player
{
    public Guid Id { get; private set; }
    public long UserId { get; private set; }
    public Guid TeamId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Login { get; private set; }
    public long? LastReviewerId { get; private set; }
    public LanguageId LanguageId { get; private set; } = default!;

    private Player()
    {
    }

    public Player(LanguageId languageId, long userId, Guid teamId, string name, string? login)
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

        LanguageId = languageId ?? throw new ArgumentNullException(nameof(languageId));
        Id = Guid.NewGuid();
        UserId = userId;
        TeamId = teamId;
        Name = name;
        Login = login;
    }
}