using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed class PlayerAsReviewer
{
    public Guid Id { get; private set; }
    public long UserId { get; private set; }
    public LanguageId LanguageId { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Login { get; private set; }

    private PlayerAsReviewer()
    {
    }

    public PlayerAsReviewer(Player player)
        : this()
    {
        if (player is null)
            throw new ArgumentNullException(nameof(player));

        Id = player.Id;
        UserId = player.UserId;
        LanguageId = player.LanguageId;
        Name = player.Name;
        Login = player.Login;
    }

    public string GetLogin() => string.IsNullOrWhiteSpace(Login) ? Name : $"@{Login}";
}