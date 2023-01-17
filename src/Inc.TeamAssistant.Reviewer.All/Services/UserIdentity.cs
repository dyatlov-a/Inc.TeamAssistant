namespace Inc.TeamAssistant.Reviewer.All.Services;

public record UserIdentity(long? UserId, string? Username)
{
    public static UserIdentity Create(long userId) => new(userId, Username: null);
    
    public static UserIdentity Create(string username) => new(UserId: null, username);
}