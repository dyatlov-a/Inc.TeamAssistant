namespace Inc.TeamAssistant.Reviewer.All.Services;

public record UserIdentity
{
    public long? UserId { get; private init; }
    public string? Username { get; private init; }

    private UserIdentity()
    {
    }
    
    public static UserIdentity Create(long userId) => new() { UserId = userId };
    
    public static UserIdentity Create(string username) => new() { Username = username };
}